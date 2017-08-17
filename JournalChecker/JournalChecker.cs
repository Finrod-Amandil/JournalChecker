/*
 * JOURNALCHECKER.CS
 * JournalChecker core module
 * 
 * Module description:
 * -------------------
 * JournalChecker.cs contains the following functionalities:
 * - Data class to represent current journal checking environment
 * - Method CollectFilesFromDirectories to gather files within search scope
 * - Method SortFiles to categorize/sort read files, create Journal objects and check for duplicates
 * - Method ReadNames to read namelist
 * - Method DoJournalCheck to summarize and arrange the journal's statuses to a table
 * - Method GenerateReport to fill HTML-Template with analysed data
 * - Various auxiliary functions
 * 
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UtilitiesCustom;

namespace JournalChecker
{
    class JournalChecker
    {
        public List<string> Dirs { get; private set; }   //List of directories to search
        public string NameListPath { get; private set; } //Path to namelist
        public string TemplatePath { get; private set; } //Path to HTML template
        public string ReportsPath { get; private set; }  //Path to where reports are stored
        public int StartYear { get; private set; }   //Start year of timespan to check
        public int StartWeek { get; private set; }   //Start calendar week of timespan to check
        public int EndYear { get; private set; }     //End year of timespan to check
        public int EndWeek { get; private set; }     //End week of timespan to check
        public int StartWeekConf { get; private set; }   //First week of work year (required to determine year of some journals)
        public List<string> Weeks { get; private set; }  //List of all weeks to check
        public static int MaxDistance { get; private set; } //Maximum Levenshtein distance (higher number = more typos are permitted in name and journal identifier)
        public IFace Iface { get; private set; } //User interface
        public List<string> Files { get; private set; }      //List of all files in configured directories
        public List<Journal> Journals { get; private set; }  //List of journals
        public List<string> Candidates { get; private set; } //List of .doc/.docx files which are either duplicates or could not be assigned to a week/owner
        public List<string> OtherFiles { get; private set; } //Files that are not .doc/.docx but are within selected folders.
        public List<Apprentice> Apprentices { get; private set; }  //List of all apprentices, read from namelist
        public int[,] JournalApprenticeTable { get; private set; } //Table that contains a status code for each week/apprentice
        public string Content { get; private set; } //Content of HTML-template
        public Dictionary<string, string> CopyTemplates { get; private set; } //List of all parts of the template that may require being copied

        private char[] _slash = new char[] { '/' }; //String-split separator shorthand

        public JournalChecker(
            ref List<string> dirs, IFace iface, ref List<string> weekList, 
            int startYear, int startWeek, int endYear, int endWeek, int startWeekConf,
            int maxDistance,
            string nameListPath, string templatePath, string reportsPath)
        {
            this.Dirs = dirs;
            this.Iface = iface;
            this.NameListPath = nameListPath;
            this.TemplatePath = templatePath;
            this.ReportsPath = reportsPath;
            this.StartYear = startYear;
            this.StartWeek = startWeek;
            this.EndYear = endYear;
            this.EndWeek = endWeek;
            this.Weeks = weekList;
            this.StartWeekConf = startWeekConf;
            MaxDistance = maxDistance;
        }

        //Put all files in the selected directories into one list
        public void CollectFilesFromDirectories()
        {
            Files = new List<string>();
            
            foreach(string dir in Dirs)
            {
                Files.AddRange(Directory.GetFiles(dir, "*", SearchOption.AllDirectories));
            }
        }

        //Catgorize/sort files, create Journal objects and detect duplicates
        public void SortFiles()
        {
            //Temporary list of journal paths
            List<string> journalFiles = new List<string>();

            Candidates = new List<string>();
            OtherFiles = new List<string>();
            string tmp = "";

            foreach(string file in Files)
            {
                tmp = file.ToLowerInvariant().Split(new char[] { '\\' }).Last();
                //If file contains "arbeitsjournal" it is safe to assume it is a journal.
                if (tmp.Contains("arbeitsjournal") || Utilities.LevenshteinDistance(tmp.Substring(0, 14), "arbeitsjournal") <= MaxDistance)
                {
                    journalFiles.Add(file);
                }
                //Else check whether file is a word file. Add it to list of potential journals
                else if (file.ToLowerInvariant().Split(new char[] { '.' }).Last().Contains("doc")) //Grabs .doc and .docx
                {
                    Candidates.Add(file);
                }
                //Else its another file.
                else
                {
                    OtherFiles.Add(file);
                }
            }

            //Read names from nameList and create list of Apprentices
            ReadNames();

            Journals = new List<Journal>();
            foreach(string journal in journalFiles)
            {
                //Create Journal objects
                Journals.Add(new Journal(journal, Apprentices, StartWeekConf));
            }

            //Clean up list of journals:
            //- Delete Journals outside queried period
            //- Move erronous journals to Candidates list
            //- Remove duplicate journals (journals with equal content and date)
            //- Move duplicate date journals to Candidates list (journals with equal dates but different content)
            //- Mark journal copies (journals with different dates but equal content)
            for (int i = 0; i < Journals.Count; i++)
            {
                Journal journal = Journals[i];
                //Check if journal is attached to week within queried period
                if (Weeks.Contains((journal.Week < 10 ? "0" + journal.Week : "" + journal.Week) + "/" + journal.Year))
                {
                    //Unidentifyable journals
                    if (journal.Status == 2)
                    {
                        Candidates.Add(journal.FilePath);
                        Journals.RemoveAt(i);
                        i--; //correct index
                    }
                    else
                    {
                        //Compare checksum of current Journal with all that were checked before
                        for (int j = 0; j < i; j++)
                        {
                            Journal journal2 = Journals[j];

                            //Make sure the journals have the same owner
                            if (journal.Owner == journal2.Owner)
                            {
                                if (journal2.Checksum.Equals(journal.Checksum))
                                {
                                    //Is date the same?
                                    if (journal2.Year == journal.Year && journal2.Week == journal.Week)
                                    {
                                        //Same content and date, remove one.
                                        Journals.RemoveAt(i);
                                        i--;
                                        break;
                                    }
                                    else
                                    {
                                        //Mark as duplicate (apprentice copied his own journal of another week)
                                        journal.SetStatus(3);
                                        journal2.SetStatus(3); //it's not clear which one is the original, so mark both.
                                        break;
                                    }
                                }
                                //if content is different, check whether dates are equal
                                else if (journal2.Year == journal.Year && journal2.Week == journal.Week)
                                {
                                    //Different content, but same date. Move file to candidates
                                    Candidates.Add(journal.FilePath);
                                    Journals.RemoveAt(i);
                                    i--; //correct index
                                    break;
                                }
                            }
                            else if (journal2.Checksum.Equals(journal.Checksum))
                            {
                                //Mark as duplicate (apprentice copied journal of other apprentice)
                                journal.SetStatus(3);
                                journal2.SetStatus(3);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    //Remove Journal if outside of queried period.
                    Journals.RemoveAt(i);
                    i--;
                }
            }

            return;
        }

        //Read name list and create Apprentice objects, sort alphabetically
        private void ReadNames()
        {
            List<string> names = new List<string>();
            Apprentices = new List<Apprentice>();
            StreamReader file = new StreamReader(NameListPath);
            string line = "";
            while ((line = file.ReadLine()) != null)
            {
                names.Add(line);
            }
            names.Sort();

            foreach(string name in names)
            {
                string[] parts = name.Split(new char[] { ';' });
                Apprentices.Add(new Apprentice(parts[1], parts[0]));
            }
            file.Close();
            return;
        }

        public void DoJournalCheck()
        {
            //Table with one cell for each Journal (rows: Apprentices, columns: Weeks). For each cell the status of the journal is noted.
            JournalApprenticeTable = new int[Apprentices.Count, Weeks.Count];

            //Initialize Table
            for (int i = 0; i < JournalApprenticeTable.GetLength(0); i++)
            {
                for (int j = 0; j < JournalApprenticeTable.GetLength(1); j++)
                {
                    JournalApprenticeTable[i, j] = 2; //Assume "Journal missing" by default.
                }
            }

            foreach (Journal journal in Journals)
            {
                int ownerID = -1; //-1 = not found (not used with Apprentices)
                int weekID = -1;  //-1 = not found

                //Find owner of Journal
                for (int i = 0; i < Apprentices.Count; i++)
                {
                    if (journal.Owner == Apprentices[i])
                    {
                        ownerID = i;
                        break;
                    }
                }

                //Find week of Journal
                for (int i = 0; i < Weeks.Count; i++)
                {
                    //Attention: journal.Week is an int, remember leading 0's!
                    if ((journal.Week + "/" + journal.Year).Equals(Weeks[i]) || ("0" + journal.Week + "/" + journal.Year).Equals(Weeks[i]))
                    {
                        weekID = i;
                        break;
                    }
                }

                //If week could not be found Journal belongs to a week outside of queried range.
                if (!(weekID == -1))
                {
                    //Set field that matches journal to status of journal
                    JournalApprenticeTable[ownerID, weekID] = journal.Status;
                }
            }

            //Set status to 4 if no journal found and date lies in the future
            int now_year = DateTime.Now.Year;
            int now_week = DateTimeFormatInfo.CurrentInfo.Calendar.GetWeekOfYear(DateTime.Now, DateTimeFormatInfo.CurrentInfo.CalendarWeekRule, DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek);
            for (int i = Weeks.Count - 1; i >= 0; i--)
            {
                //"future" == week is larger than current week and year is larger or equal to current week.
                if (int.Parse(Weeks[i].ToString().Split(new char[] { '/' }) [0]) > now_week &&
                    int.Parse(Weeks[i].ToString().Split(new char[] { '/' }) [1]) == now_year ||
                    int.Parse(Weeks[i].ToString().Split(new char[] { '/' })[1]) > now_year)
                {
                    for (int j = 0; j < Apprentices.Count; j++)
                    {
                        if (JournalApprenticeTable[j, i] == 2)
                        {
                            JournalApprenticeTable[j, i] = 4;
                        }
                    }
                }
            }
        }

        public void GenerateReport()
        {
            //Read entire template
            Content = File.ReadAllText(TemplatePath);

            //Hashmap with all copy templates
            CopyTemplates = new Dictionary<string, string>();
            int index = -1;
            while ((index = Content.IndexOf("<!--CPS_", index + 1)) != -1)
            {
                int tmp_start = Content.IndexOf("_", index) + 1;
                int tmp_end = Content.IndexOf("-", tmp_start);
                string tmp_key = Content.Substring(tmp_start, tmp_end - tmp_start);
                CopyTemplates[tmp_key] = GetCopyTemplate(tmp_key, Content);
            }

            //Set start and end date
            InsertVar("startWeek", "KW" + (StartWeek < 10 ? "0" + StartWeek : "" + StartWeek) + "/" + StartYear);
            InsertVar("endWeek", "KW" + (EndWeek < 10 ? "0" + EndWeek : "" + EndWeek) + "/" + EndYear);

            //List of searched directories
            foreach (string dir in Dirs)
            {
                //Extend relative paths to absolute paths
                if (dir.Substring(0, 2) == ".\\")
                {
                    InsertVar("directoryDisplay", AppDomain.CurrentDomain.BaseDirectory + dir.Substring(2));
                    InsertVar("directory", FilePathToUrl(AppDomain.CurrentDomain.BaseDirectory + dir.Substring(2)));
                }
                else
                {
                    InsertVar("directoryDisplay", dir);
                    InsertVar("directory", FilePathToUrl(dir));
                }
                CopyTemplate("directoryList");
            }
            //Replace redundant last template copy
            Content = Content.Replace(CopyTemplates["directoryList"], "");

            //Main table
            //Header
            string curr_week = "";
            int curr_year = EndYear;
            int prev_year = EndYear;
            int yearColCount = 0;

            //weeks are sorted in ascending order, in table they will be in descending order
            for (int i = Weeks.Count - 1; i >= 0; i--)
            {

                //add new week cell
                curr_week = Weeks[i].Split(new char[] { '/' })[0];
                InsertVar("mainTableWeek", curr_week);

                if (i != 0)
                {
                    CopyTemplate("mainTableWeek");
                }

                curr_year = int.Parse(Weeks[i].ToString().Split(new char[] { '/' })[1]);

                //if a new year is found, make new cell
                if (curr_year != prev_year || i == 0)
                {
                    //Fix index for last cell
                    if (i == 0)
                    {
                        yearColCount++;
                    }

                    //fill previously added copy
                    InsertVar("mainTableYearColCount", yearColCount.ToString());
                    InsertVar("mainTableYear", prev_year.ToString());

                    //add new empty copy
                    if (i != 0)
                    {
                        CopyTemplate("mainTableYear");
                    }

                    yearColCount = 0;
                    prev_year = curr_year;
                }
                yearColCount++;
            }

            //Main table
            //Content
            for (int i = 0; i < JournalApprenticeTable.GetLength(0); i++)
            {
                //Set content of current row
                InsertVar("mainTableApprenticeName", Apprentices[i].FullName);
                for (int j = JournalApprenticeTable.GetLength(1) - 1; j >= 0; j--)
                {
                    //Set current cell
                    InsertVar("mainTableJournalStatus", JournalApprenticeTable[i,j].ToString());

                    if (JournalApprenticeTable[i,j] != 2 && JournalApprenticeTable[i,j] != 4)
                    {
                        string path = GetJournal(Apprentices[i], Weeks[j]).FilePath;
                        
                        for (int k = 0; k < path.Length; k++)
                        {
                            if (path.Substring(k,1).Equals("\\"))
                            {
                                path = path.Insert(k, "\\");
                                k++;
                            }
                        }
                        path = FilePathToUrl(path);
                        path = path.Substring(0, path.LastIndexOf("\\\\"));

                        InsertVar("mainTableCellCmd", " onclick=\"window.open('" + path + "');\"");
                    }
                    else
                    {
                        InsertVar("mainTableCellCmd", "");
                    }

                    if (j != 0)
                    {
                        CopyTemplate("mainTableCell");
                    }
                }

                if (i != JournalApprenticeTable.GetLength(0) - 1)
                {
                    CopyTemplate("mainTableRow");
                }
            }

            //Missing journals
            //Write data to temporary list for sorting
            List<List<string>> tmp_table_data = new List<List<string>>();
            tmp_table_data.Add(new List<string>()); //First column
            tmp_table_data.Add(new List<string>()); //Second column
            for (int i = 0; i < JournalApprenticeTable.GetLength(0); i++)
            {
                for (int j = 0; j < JournalApprenticeTable.GetLength(1); j++)
                {
                    if (JournalApprenticeTable[i,j] == 2)
                    {
                        //Sort list when inserting
                        int k = 0;

                        //Sort by name
                        while (k < tmp_table_data[0].Count &&
                               String.Compare(Apprentices[i].FullName, tmp_table_data[0][k]) > 0)
                            k++;

                        //Sort by year
                        while (k < tmp_table_data[0].Count &&
                               String.Compare(Apprentices[i].FullName, tmp_table_data[0][k]) == 0 && //while names are equal
                               String.Compare(Weeks[j].Split(_slash).Last(),tmp_table_data[1][k].Split(_slash).Last()) > 0) //Year of current journal equal or larger than last year
                            k++;

                        //Sort by week
                        while (k < tmp_table_data[0].Count &&
                               String.Compare(Apprentices[i].FullName, tmp_table_data[0][k]) == 0 && //while names are equal
                               String.Compare(Weeks[j].Split(_slash).Last(), tmp_table_data[1][k].Split(_slash).Last()) == 0 && //Year of current journal equal or larger than last year
                               String.Compare(Weeks[j].Substring(2, 2), tmp_table_data[1][k].Substring(2, 2)) > 0) //Compare weeks
                            k++;


                        tmp_table_data[0].Insert(k, Apprentices[i].FullName);
                        tmp_table_data[1].Insert(k, Weeks[j]);
                    }
                }
            }

            for (int i = 0; i < tmp_table_data[0].Count; i++) //all rows
            {
                InsertVar("missingJournalApprenticeName", tmp_table_data[0][i]);
                InsertVar("missingJournalWeek", "KW" + tmp_table_data[1][i]);
                CopyTemplate("missingJournalRow");
            }
            //Replace redundant last template copy
            Content = Content.Replace(CopyTemplates["missingJournalRow"], "");

            //List of other docs (candidates for missing journals)
            List<string> tmp = new List<string>();
            tmp.AddRange(Candidates);
            tmp.AddRange(OtherFiles);
            foreach (string otherDoc in tmp)
            {
                InsertVar("otherDocFilePathDisplay", otherDoc.Substring(0, otherDoc.LastIndexOf('\\') + 1));
                InsertVar("otherDocFilePath", FilePathToUrl(otherDoc.Substring(0, otherDoc.LastIndexOf('\\') + 1)));
                InsertVar("otherDocFileName", otherDoc.Substring(otherDoc.LastIndexOf('\\') + 1));
                CopyTemplate("otherDocRow");
            }
            //Replace redundant last template copy
            Content = Content.Replace(CopyTemplates["otherDocRow"], "");

            //Duplicates
            //Jagged 2D-dynamic array; each sub-array is list of all journals with common SHA1-Checksum
            List<List<Journal>> duplicates = new List<List<Journal>>();
            foreach (Journal journal in Journals)
            {
                if (journal.Status == 3)
                {
                    //Go through all sub-lists and see whether current checksum has already been entered to list
                    Boolean added = false;
                    for (int i = 0; i < duplicates.Count; i++)
                    {
                        //Append journal to existing sub-list
                        if (duplicates[i][0].Checksum.Equals(journal.Checksum))
                        {
                            duplicates[i].Add(journal);
                            added = true;
                            break;
                        }
                    }
                    if (!added) //If journal was not added to any of the existing sub-lists, make new list
                    {
                        duplicates.Add(new List<Journal>());
                        duplicates.Last().Add(journal);
                    }
                }
            }

            for (int i = 0; i < duplicates.Count; i++)
            {
                InsertVar("duplicateRowCount", "" + duplicates[i].Count);
                InsertVar("duplicateRowNum", "" + (i + 1));
                for (int j = 0; j < duplicates[i].Count; j++)
                {
                    Journal duplicate_journal = duplicates[i][j];
                    InsertVar("duplicateApprenticeName", duplicate_journal.Owner.FullName);
                    InsertVar("duplicateWeek", "KW" + ((duplicate_journal.Week < 10) ? ("0" + duplicate_journal.Week) : ("" + duplicate_journal.Week)) + "/" + duplicate_journal.Year);
                    InsertVar("duplicateFilePathDisplay", duplicate_journal.Directory);
                    InsertVar("duplicateFilePath", FilePathToUrl(duplicate_journal.Directory));
                    InsertVar("duplicateFileName", duplicate_journal.FileName);
                    InsertVar("duplicateChecksum", duplicate_journal.Checksum);
                    CopyTemplate("duplicateRow");
                    if (j != duplicates[i].Count - 1)
                    {
                        Content = Content.Replace(CopyTemplates["duplicateGroup"], "");
                    }
                }
            }
            Content = Content.Replace(CopyTemplates["duplicateRow"], "");


            //Write data to temporary list for sorting
            tmp_table_data = new List<List<string>>();
            for (int i = 0; i < 7; i++) tmp_table_data.Add(new List<string>()); //Initialise columns

            foreach (Journal journal in Journals) 
            {
                if (journal.Status == 1)
                {
                    string fileNameCorrected = "arbeitsjournal-" +
                                               Utilities.ReplaceSpecialChars(journal.Owner.LastName.ToLowerInvariant()) + "-" +
                                               Utilities.ReplaceSpecialChars(journal.Owner.FirstName.ToLowerInvariant()) + "-" +
                                               journal.Year + "-" +
                                               ((journal.Week < 10) ? ("0" + journal.Week) : ("" + journal.Week)) + ".docx";
                    string tmp_week = "KW" + ((journal.Week < 10) ? ("0" + journal.Week) : ("" + journal.Week)) + "/" + journal.Year;

                    //Sort list when inserting
                    int i = 0;

                    //Sort by name
                    while (i < tmp_table_data[0].Count && 
                           String.Compare(journal.Owner.FullName, tmp_table_data[0][i]) > 0)
                        i++;

                    //Sort by year
                    while (i < tmp_table_data[0].Count && 
                           String.Compare(journal.Owner.FullName, tmp_table_data[0][i]) == 0 && //while names are equal
                           journal.Year > int.Parse(tmp_table_data[1][i].Split(_slash).Last())) //Year of current journal equal or larger than last year
                        i++;

                    //Sort by week
                    while (i < tmp_table_data[0].Count &&
                           String.Compare(journal.Owner.FullName, tmp_table_data[0][i]) == 0 && //while names are equal
                           journal.Year == int.Parse(tmp_table_data[1][i].Split(_slash).Last()) && //Year of current journal equal or larger than last year
                           journal.Week > int.Parse(tmp_table_data[1][i].Substring(2, 2))) //Compare weeks
                        i++;

                    tmp_table_data[0].Insert(i, journal.Owner.FullName);
                    tmp_table_data[1].Insert(i, tmp_week);
                    tmp_table_data[2].Insert(i,journal.Directory);
                    tmp_table_data[3].Insert(i, FilePathToUrl(journal.Directory));
                    tmp_table_data[4].Insert(i, fileNameCorrected);
                    tmp_table_data[5].Insert(i, journal.FileName);
                    tmp_table_data[6].Insert(i, "- " + journal.ErrMsg.TrimEnd(new char[] { ';' }).Replace(";", "<br>- "));
                }
            }

            for (int i = 0; i < tmp_table_data[0].Count; i++) //all rows
            {
                InsertVar("inconsistentFileApprenticeName", tmp_table_data[0][i]);
                InsertVar("inconsistentFileWeek", tmp_table_data[1][i]);
                InsertVar("inconsistentFilePathDisplay", tmp_table_data[2][i]);
                InsertVar("inconsistentFilePath", tmp_table_data[3][i]);
                InsertVar("inconsistentFileNameCorrected", tmp_table_data[4][i]);
                InsertVar("inconsistentFileName", tmp_table_data[5][i]);
                InsertVar("inconsistentFileMsg", tmp_table_data[6][i]);
                CopyTemplate("inconsistentFileRow");
            }

            Content = Content.Replace(CopyTemplates["inconsistentFileRow"], "");

            string htmlFilePath = ReportsPath + "\\report-" + string.Format("{0:yyyy-MM-dd_HH-mm-ss}", DateTime.Now) + ".html";
            File.WriteAllText(htmlFilePath, Content);
            System.Diagnostics.Process.Start(htmlFilePath);
        }

        private static string GetCopyTemplate(string id, string content)
        {
            string startTag = "<!--CPS_" + id + "-->";
            string endTag = "<!--CPE_" + id + "-->";
            int startIndex = content.IndexOf(startTag) + startTag.Length;
            int endIndex = content.IndexOf(endTag);

            return (content.Substring(startIndex, endIndex - startIndex));
        }

        private void CopyTemplate(string id)
        {
            Content = Content.Insert(Content.LastIndexOf("<!--CPE_" + id + "-->"), CopyTemplates[id]);
        }

        private void InsertVar(string id, string val)
        {
            Content = Content.Replace("$" + id + "", val);
        }

        private Journal GetJournal(Apprentice apprentice, string week)
        {
            foreach (Journal journal in Journals)
            {
                if (journal.Owner == apprentice && ((journal.Week + "/" + journal.Year).Equals(week) || ("0" + journal.Week + "/" + journal.Year).Equals(week)))
                {
                    return journal;
                }
            }

            return null;
        }

        private static string FilePathToUrl(string filePath)
        {
            string[,] replace = new string[,] { 
                                              { "%"  ," "  , "<"  , ">"  ,"#"  ,"{"  ,"}"  ,"|"  ,"^"  ,"~"  ,"["  ,"]"  ,"`"  ,";"  ,"/"  ,"?"  ,"@"  ,"="  ,"&"  ,"$"   },
                                              { "%25","%20", "%3C", "%3E","%23","%7B","%7D","%7C","%5E","%7E","%5B","%5D","%60","%3B","%2F","%3F","%40","%3D","%26","%24" },
                                              };
            for (int i = 0; i < replace.GetLength(1); i++)
            {
                filePath = filePath.Replace(replace[0, i], replace[1, i]);
            }

            return filePath;
        }
    }
}
