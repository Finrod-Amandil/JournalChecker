/*
 * JOURNAL.CS
 * JournalChecker core module
 * 
 * Module description:
 * -------------------
 * Journal.cs contains the following functionalities:
 * - Data class to represent a work Journal
 * - Constructor contains functionalities to determine a journal's user and week
 * - Auxiliary function to set journal status
 * - SHA1-Checksum generator
 * 
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UtilitiesCustom;

namespace JournalChecker
{
    class Journal
    {
        public string FilePath { get; private set; }
        public string Directory { get; private set; }
        public string FileName { get; private set; }

        public int Week { get; private set; }
        public int Year { get; private set; }

        public Apprentice Owner { get; private set; }
        public string Checksum { get; private set; }
        public int Status { get; private set; }
        /*
         * Status = 0: No errors detected
         * Status = 1: Errors detected, but none fatal
         * Status = 2: Fatal error occurred, could not recreate missing data (External: Journal missing)
         * Status = 3: Journal is connected to unique week but it's content is a duplicate of another journal
         * Status = 4: External only: Date lies in the future
         */
        public string ErrMsg { get; private set; }

        public Journal(string path, List<Apprentice> apprentices, int startWeek)
        {
            Status = 0; //Default assume no errors
            ErrMsg = "";
            FilePath = path;
            Owner = null;
            FileName = FilePath.Substring(FilePath.LastIndexOf('\\') + 1);
            Directory = FilePath.Substring(0, FilePath.LastIndexOf('\\') + 1);

            string filename_normalized = FileName.Split(new char[] { '.' }).First();

            //Check / replace special chars
            if (filename_normalized != Utilities.ReplaceSpecialChars(filename_normalized))
            {
                filename_normalized = Utilities.ReplaceSpecialChars(filename_normalized);
                SetStatus(1);
                ErrMsg = ErrMsg + "Dateiname darf nur alphanumerische Zeichen und Bindestriche enthalten.;";
            }

            //Check / replace capitals
            if (!(filename_normalized.ToLowerInvariant()).Equals(filename_normalized))
            {
                filename_normalized = filename_normalized.ToLowerInvariant();
                //SetStatus(1);
                //ErrMsg = ErrMsg + "Dateiname darf keine Grossbuchstaben enthalten.;";
            }

            string[] parts = filename_normalized.Split(new char[] { '-', '_', ' ', '/', '\\', '|', '.', ',' });
            //Check and split on separators
            for (int i = 0; i < parts.Length; i++)
            {
                if (parts.Length != filename_normalized.Split(new char[] { '-' }).Length || !(parts[i].Equals(filename_normalized.Split(new char[] { '-' })[i])))
                {
                    SetStatus(1);
                    ErrMsg = ErrMsg + "Nur Bindestriche sind erlaubt als Trennzeichen.;";
                    break;
                }
            }
            
            //Avoid IndexOutOfBoundsException by stretching array up to five entries if necessary.
            if (parts.Length < 5)
            {
                string[] tmp = new string[] { "", "", "", "", "" };
                for (int i = 0; i < parts.Length; i++)
                {
                    tmp[i] = parts[i];
                }

                parts = new string[5];
                Array.Copy(tmp, parts, 5);
            }

            //default file format = arbeitsjournal-lastname-firstname-YYYY-WW.docx

            //Check for typos in "arbeitsjournal" (LevenshteinDistance was already checked in JournalChecker.cs)
            if (!parts[0].Equals("arbeitsjournal"))
            {
                SetStatus(1);
                ErrMsg = ErrMsg + "Arbeitsjournal-Identifikator \"arbeitsjournal\" hat Schreibfehler.;";
            }

            //Find Owner of file
            //Best case: last name is at right place and last and first name match
            foreach (Apprentice apprentice in apprentices)
            {
                if(Utilities.ReplaceSpecialChars(apprentice.LastName.ToLowerInvariant()) == parts[1] && 
                   Utilities.ReplaceSpecialChars(apprentice.FirstName.ToLowerInvariant()) == parts[2])
                {
                    Owner = apprentice;
                    break;
                }
            }

            //If owner could not be identified, try other ways to find the names.
            //Try to find a last name and first name in other places.
            if (Owner == null)
            {
                foreach (Apprentice apprentice in apprentices)
                {
                    if (filename_normalized.Contains(Utilities.ReplaceSpecialChars(apprentice.LastName).ToLowerInvariant()) && 
                        filename_normalized.Contains(Utilities.ReplaceSpecialChars(apprentice.FirstName).ToLowerInvariant()))
                    {
                        Owner = apprentice;
                        ErrMsg = ErrMsg + "Vor- und/oder Nachname befinden sich an unerwarteten Stellen im Dateiname.;";
                        SetStatus(1);
                    }
                }
            }

            //Try to find names with typos in filename
            if (Owner == null)
            {
                foreach (string part in parts)
                {
                    foreach (Apprentice apprentice in apprentices)
                    {
                        if (Utilities.LevenshteinDistance(part, Utilities.ReplaceSpecialChars(apprentice.LastName).ToLowerInvariant()) <= JournalChecker.MaxDistance &&
                            filename_normalized.Contains(Utilities.ReplaceSpecialChars(apprentice.FirstName).ToLowerInvariant()) ||
                            Utilities.LevenshteinDistance(part, Utilities.ReplaceSpecialChars(apprentice.FirstName).ToLowerInvariant()) <= JournalChecker.MaxDistance &&
                            filename_normalized.Contains(Utilities.ReplaceSpecialChars(apprentice.LastName).ToLowerInvariant()))
                        {
                            Owner = apprentice;
                            ErrMsg = ErrMsg + "Vor- oder Nachname hat Schreibfehler.;";
                            SetStatus(1);
                        }
                    }
                }
            }

            //Try to find a last name and first name in entire filepath
            if (Owner == null)
            {
                foreach (Apprentice apprentice in apprentices)
                {
                    if (Utilities.ReplaceSpecialChars(FilePath).ToLowerInvariant().Contains(Utilities.ReplaceSpecialChars(apprentice.LastName).ToLowerInvariant()) &&
                        Utilities.ReplaceSpecialChars(FilePath).ToLowerInvariant().Contains(Utilities.ReplaceSpecialChars(apprentice.FirstName).ToLowerInvariant()))
                    {
                        Owner = apprentice;
                        ErrMsg = ErrMsg + "Vor- und Nachname befinden sich im Dateipfad oder Dateiname, aber nicht an der erwarteten Stelle.;";
                        SetStatus(1);
                    }
                }
            }

            //Try to find a last name match only.
            if (Owner == null)
            {
                foreach (Apprentice apprentice in apprentices)
                {
                    if (filename_normalized.Contains(Utilities.ReplaceSpecialChars(apprentice.LastName).ToLowerInvariant()))
                    {
                        Owner = apprentice;
                        ErrMsg = ErrMsg + "Besitzer wurde nur durch Nachname bestimmt.";
                        SetStatus(1);
                    }
                }
            }

            //If Owner still not identified, mark Journal as broken
            if (Owner == null)
            {
                ErrMsg = ErrMsg + "FATAL: Owner could not be identified;";
                SetStatus(2);
            }

            //---------------------------------------------
            //Find year and week

            //Initialise
            this.Week = 0;
            this.Year = 0;

            //Find week
            //Look at expected position
            if (int.TryParse(parts[4], out int week) && week > 0 && week <= 53)
            {
                this.Week = week;
            }
            else
            {
                //If part 4 was not a valid week, maybe another part is.
                foreach (string part in parts)
                {
                    //Take last found 2-digit number, if there's more it's most likely the year (auto-overwrite in that case)
                    if (part.Length == 2 && int.TryParse(part, out week) && week > 0 && week <= 53)
                    {
                        this.Week = week;
                    }
                }

                //If a value has been found for week, take that
                if (this.Week != 0)
                {
                    ErrMsg = ErrMsg + "Angabe der Woche befindet sich an unerwarteter Stelle im Dateinamen.;";
                    SetStatus(1);
                }

                //If week still not found, try looking for 1-digit numbers
                if (this.Week == 0)
                {
                    foreach (string part in parts)
                    {
                        if (int.TryParse(part, out week) && part.Length == 1)
                        {
                            this.Week = week;
                            ErrMsg = ErrMsg + "Wochenangabe war nur eine Ziffer lang, muss immer zweistellig sein.;";
                            SetStatus(1);
                        }
                    }
                }

                //If week is still not found, try to find a number being part of an alphanumerical string token
                if (this.Week == 0)
                {
                    int tmp = 0;
                    foreach (string part in parts)
                    {
                        //Only proceed if entire token cannot be parsed as number (avoid mistaking year for week)
                        if (!int.TryParse(part, out tmp))
                        {
                            for (int i = 0; i < part.Length; i++)
                            {
                                //Try parsing a 1-digit number
                                if (int.TryParse(part.Substring(i, 1), out tmp))
                                {
                                    //1-digit worked, try a 2-digit (make sure there's two chars left on the token)
                                    if (part.Length - i > 1 && int.TryParse(part.Substring(i, 2), out tmp))
                                    {
                                        //validate 2-digit week
                                        if (tmp > 0 && tmp < 53)
                                        {
                                            this.Week = tmp;
                                            ErrMsg = ErrMsg + "Wochenangabe wurde mit nicht-numerischen Zeichen gemischt.;";
                                            SetStatus(1);
                                            break;
                                        }
                                    }
                                    //no two consecutive digits were found, validate 1-digit week
                                    else
                                    {
                                        this.Week = tmp;
                                        ErrMsg = ErrMsg + "Wochenangabe wurde mit nicht-numerischen Zeichen gemischt.;";
                                        ErrMsg = ErrMsg + "Wochenangabe war nur eine Ziffer lang, muss immer zweistellig sein.;";
                                        SetStatus(1);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            //Find year
            //Look at expected position
            if (int.TryParse(parts[3], out int year) && year > 0 && parts[3].Length == 4)
            {
                this.Year = year;
            }
            else
            {
                //Try to find a 4-digit number somewhere else
                foreach (string part in parts)
                {
                    if (part.Length == 4 && int.TryParse(part, out year) && year > 0)
                    {
                        this.Year = year;
                        ErrMsg = ErrMsg + "Angabe des Jahres befindet sich an unerwarteter Stelle im Dateinamen.;";
                        SetStatus(1);
                    }
                }
            }

            //If year not found yet, try finding it as a 2-digit number
            if (this.Year == 0)
            { 
                //Check if there's more than 1 2-digit numbers
                int countNums = 0;
                int tmp = 0;
                foreach (string part in parts)
                {
                    if (part.Length == 2 && int.TryParse(part, out tmp))
                    {
                        countNums++;
                    }

                    //Save relevant number from overriding
                    if (countNums == 1)
                    {
                        year = tmp;
                    }

                    //Apply first number found if there is a second one.
                    if (countNums == 2)
                    {
                        this.Year = (year <= (DateTime.Now.Year - 2000)) ? year + 2000 : year + 1900;
                        ErrMsg = ErrMsg + "Jahresangabe war nur zweistellig, es wurde " + Year.ToString().Substring(0,2) + "## angenommen.;";
                        SetStatus(1);
                        break; //Discard any further numbers
                    }
                }
            }

            //If year still not found, try finding it from parent directory names
            if (this.Year == 0)
            {
                List<int> yearCandidates = new List<int>();
                foreach (string dir in FilePath.Split(new char[] { '\\' }))
                {
                    //Iterate through folder name, look at the group of 4 chars starting from current position and try to parse them as int.
                    for (int i = 0; i < dir.Length - 3; i++)
                    {
                        if (int.TryParse(dir.Substring(i, 4), out year) && year > 0)
                        {
                            yearCandidates.Add(year);
                        }
                    }
                }
                //If only one year was found use that.
                if (yearCandidates.Count == 1)
                {
                    this.Year = year;
                    ErrMsg = ErrMsg + "Jahresangabe befindet sich im Dateipfad aber nicht im Dateinamen.;";
                    SetStatus(1);
                }
                //If two consecutive years were found, find better one according to configured start week.
                else if (yearCandidates.Count == 2 && yearCandidates[1] - yearCandidates[0] == 1)
                {
                    if (this.Week > startWeek)
                    {
                        this.Year = yearCandidates[0];
                    }
                    else
                    {
                        this.Year = yearCandidates[1];
                    }
                    ErrMsg = ErrMsg + "Jahresangabe befindet sich im Dateipfad aber nicht im Dateinamen.;";
                    SetStatus(1);
                }
                //Else use newest date
                else
                {
                    yearCandidates.Sort();
                    this.Year = yearCandidates.Last();
                    ErrMsg = ErrMsg + "Jahresangabe befindet sich im Dateipfad aber nicht im Dateinamen.;";
                    SetStatus(1);
                }
            }

            //If not both values could be found, mark journal as broken
            if (this.Week == 0 || this.Year == 0)
            {
                ErrMsg = ErrMsg + "FATAL: Week and/or year could not be determined;";
                SetStatus(2);
            }

            GenerateChecksum();
        }

        public void SetStatus(int status)
        {
            //Make sure higher statuses get not overwritten
            if (this.Status < status) this.Status = status;
        }

        public void SetStatus(int status, Boolean force)
        {
            if (force)
            {
                this.Status = status;
            }
            else
            {
                SetStatus(status);
            }
        }

        private void GenerateChecksum()
        {
            using (FileStream fs = new FileStream(FilePath, FileMode.Open))
            using (BufferedStream bs = new BufferedStream(fs))
            {
                using (SHA1Managed sha1 = new SHA1Managed())
                {
                    byte[] hash = sha1.ComputeHash(bs);
                    StringBuilder formatted = new StringBuilder(2 * hash.Length);
                    foreach (byte b in hash)
                    {
                        formatted.AppendFormat("{0:X2}", b);
                    }
                    Checksum = formatted.ToString();
                }
            }
        }
    }
}
