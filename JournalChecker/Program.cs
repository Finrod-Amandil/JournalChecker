/*
 * PROGRAM.CS
 * JournalChecker main module
 * 
 * Author: Zahler Severin
 * Version: 1.1
 * Last change: August 17th, 2017
 * 
 * Overall program description:
 * ----------------------------
 * JournalChecker analyses work journals of apprentices and summarizes which journals have been submitted and which ones are still missing.
 * All files within the directories that have been specified in the JournalChecker.exe.config file will be respected. The journals should
 * follow the nomenclature "Arbeitsjournal-lastname-firstname-calendarweek-year.doc(x)". Malformed files will be tried to be appointed to the
 * correct person and week, if successful a matching warning is shown.
 * 
 * The identified journals are listed in a table within a HTML-logfile.
 * 
 * 
 * Module description:
 * -------------------
 * Program.cs contains the following functionalities:
 * - Check config file for invalid entries
 * - Check user arguments / GUI input
 * - Set defaults where no arguments are given
 * - Output status and error messages
 * - Initiate JournalChecker module and its methods
 * 
 *
 * Allowed Syntax:
 * ---------------
 * JournalChecker.exe
 * JournalChecker.exe /d
 * JournalChecker.exe #        (from calendar week #)
 * JournalChecker.exe #/#      (from calendar week #, year #)
 * JournalChecker.exe # #      (from calendar week # to calendar week #)
 * JournalChecker.exe #/# #/#  (from calendar week #, year # to calendar week #, year #)
 * JournalChecker.exe /?        
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using UtilitiesCustom;

namespace JournalChecker
{
    static class Program
    { 
        [STAThread]
        static void Main(string[] args)
        {
            //If argument is flag for help, print help text and abort.
            if (args.Length > 0 && args[0] == "/?")
            {
                PrintHelp();
                return;
            }

            //Determine User Interface, without arguments GUI is shown.
            IFace iface = (args.Length == 0) ? IFace.GUI : IFace.CMD;

            //Verify configurations
            List<string> dirs = new List<string>();
            int confStatus = VerifyConfig(ref dirs, out string confErr, out string nameListPath, out string templatePath, out string reportsPath, out int startWeekConf, out int maxDistance);
            if (confStatus != 0 && iface != IFace.GUI)
            {
                System.Console.WriteLine(confErr);
                return; //Abort if erronous
            }

            //Verify console arguments resp. set default values
            int argStatus = VerifyArgs(args, startWeekConf, out string argErr, out int[] weeks, out int[] years, out List<string> weekList);
            if (argStatus != 0 && iface != IFace.GUI)
            {
                System.Console.WriteLine(argErr);
                PrintHelp();
                return; //Abort if erronous
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form_Start form = new Form_Start(weeks, years, argErr, confErr);

            if (iface == IFace.GUI)
            {
                do
                {
                    //Start GUI
                    form = new Form_Start(weeks, years, argErr, confErr);
                    Application.Run(form);

                    //Fetch arguments from form (Config is set in Form_Start.cs)
                    args = form.GetArgs();

                    //If no values are to be found in array user has closed GUI with "close" button
                    if (args[0] == null || args[1] == null) return;

                    //Verify GUI arguments and config and update weekList
                    confStatus = VerifyConfig(ref dirs, out confErr, out nameListPath, out templatePath, out reportsPath, out startWeekConf, out maxDistance);
                    argStatus = VerifyArgs(args, startWeekConf, out argErr, out weeks, out years, out weekList);
                } while (confStatus == 1 || argStatus == 1); //Repeat until no errors persist.
            }

            Console.WriteLine("Validierung erfolgreich! Starte JournalChecker mit den folgenden Einstellungen:\n");

            Console.WriteLine("Zu prüfende Verzeichnisse:");
            foreach (string dir in dirs)
            {
                Console.Out.WriteLine("* " + dir);
            }
            Console.WriteLine();
            Console.WriteLine("Namensliste: " + nameListPath + "\nHTML-Template: " + templatePath + "\nReporte werden abgelegt unter " + reportsPath);
            Console.WriteLine("Start: KW" + (weeks[0] < 10 ? "0" + weeks[0] : "" + weeks[0]) + "/" + years[0] + "\nEnde: KW" + weeks[1] + "/" + years[1] + "\n\n\n");

            Console.WriteLine("================================================");
            Console.WriteLine("Journal-Check wird durchgeführt, bitte warten...");
            Console.WriteLine("================================================");

            JournalChecker jc = new JournalChecker(ref dirs, iface, ref weekList, years[0], weeks[0], years[1], weeks[1], startWeekConf, maxDistance, nameListPath, templatePath, reportsPath);

            //Deploy JournalChecker methods
            jc.CollectFilesFromDirectories();
            jc.SortFiles();
            jc.DoJournalCheck();
            jc.GenerateReport();
        }

        //------------------------------------------------------------------------------------------------------

        private static int VerifyConfig(ref List<string> dirs, out string confErr, out string nameListPath, out string templatePath, out string reportsPath, out int startWeekConf, out int maxDistance)
        {
            //Verify directories
            string[] dir_tmp = System.Configuration.ConfigurationManager.AppSettings["dirs"].Split(new char[] { ';' });
            confErr = "";
            nameListPath = "";
            templatePath = "";
            reportsPath = "";
            startWeekConf = 0;
            maxDistance = 0;
            int retVal = 0;

            //Empty list of dirs (Else GUI adds directories twice)
            dirs.Clear();

            //Iterate through specified directories, check whether they are valid and then add them to list of valid directories.
            foreach (string dir in dir_tmp)
            {
                if (dir == "")
                {
                    continue;
                }
                try
                {
                    FileAttributes attr = File.GetAttributes(dir);
                    if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                    {
                        dirs.Add(dir);
                    }
                    else
                    {
                        confErr = confErr + "Konnte Verzeichnis nicht finden: " + dir + ", wird übersprungen.\n";
                    }
                }
                catch (FileNotFoundException fnfe)
                {
                    confErr = confErr + "Konnte Verzeichnis nicht finden: " + dir + ", wird übersprungen.\n";
                }
            }

            //Abort if no valid directories could be found, else continue with valid ones.
            if (dirs.Count == 0)
            {
                confErr = confErr + "Kein gültiges Verzeichnis gefunden.\n";
                retVal = 1;
            }

            //Verify namelist
            nameListPath = System.Configuration.ConfigurationManager.AppSettings["namelist"];
            if (nameListPath == "")
            {
                confErr = confErr + "Es wurde kein Pfad für die Namensliste konfiguriert.\n";
            }
            //Check if file exists and extension is .txt
            else if (!(File.Exists(nameListPath) && nameListPath.Split(new char[] { '.' }).Last() == "txt"))
            {
                confErr = confErr + "Namensliste konnte unter Pfad " + nameListPath + " nicht gefunden werden oder Datei ist fehlerhaft.\n";
                retVal = 1;
            }

            //Verify template path
            templatePath = System.Configuration.ConfigurationManager.AppSettings["report-template"];
            if (templatePath == "")
            {
                confErr = confErr + "Es wurde kein Pfad für das HTML-Template konfiguriert.\n";
            }
            //Check if file exists and extension is .html
            else if (!(File.Exists(templatePath) && templatePath.Split(new char[] { '.' }).Last() == "html"))
            {
                confErr = confErr + "HTML-Report Template konnte unter Pfad " + templatePath + " nicht gefunden werden oder Datei ist fehlerhaft.\n";
                retVal = 1;
            }

            //Verify records save location
            reportsPath = System.Configuration.ConfigurationManager.AppSettings["reports-location"];
            if (reportsPath == "")
            {
                confErr = confErr + "Es wurde kein Verzeichnis für die Ablage der Reporte konfiguriert.\n";
                retVal = 1;
            }
            else
            {
                try
                {
                    FileAttributes rep_attr = File.GetAttributes(reportsPath);
                    if ((rep_attr & FileAttributes.Directory) != FileAttributes.Directory)
                    {
                        confErr = confErr + "Konnte Verzeichnis für die Ablage der Reporte nicht finden: " + reportsPath + ".\n";
                    }
                }
                catch (FileNotFoundException fnfe)
                {
                    confErr = confErr + "Konnte Verzeichnis für die Ablage der Reporte nicht finden: " + reportsPath + ".\n";
                }
            }

            //Verify and convert default start week
            string tmp = System.Configuration.ConfigurationManager.AppSettings["first-calendar-week"];
            if (!(int.TryParse(tmp, out startWeekConf) && startWeekConf > 0 && startWeekConf <= 53))
            {
                confErr = confErr + "Konfigurierte Startwoche des Arbeitsjahres ist fehlerhaft: " + tmp + ".\n";
                confErr = confErr + "Bitte Konfiguration auf Fehler überprüfen.\n";
                retVal = 1;
            }

            //Verify and convert max levenshtein distance
            tmp = System.Configuration.ConfigurationManager.AppSettings["max-levenshtein-distance"];
            if (!(int.TryParse(tmp, out maxDistance) && maxDistance > 0))
            {
                confErr = confErr + "Konfigurierte maximal zulässige Levenshtein-Distanz ist ungültig: " + tmp + ".\n";
                confErr = confErr + "Bitte Konfiguration überpfrüfen, verwende Standard-Wert 0.\n";
            }
            return retVal;

        }

        //------------------------------------------------------------------------------------

        private static int VerifyArgs(string[] args, int startWeekConf, out string argErr, out int[] weeks, out int[] years, out List<string> weekList)
        {
            //Initialise out variables
            argErr = "";
            weeks = new int[2];
            years = new int[2];
            weekList = new List<string>();
            int retVal = 0;

            //Intialise current time and week format
            DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
            DateTime now = DateTime.Now;
            Calendar cal = dfi.Calendar;

            int lastWeekOfYear = 0;

            //If argument == /? do nothing
            if (args.Length > 0 && args[0] == "/?")
            {
                return 2;
            }

            //If no arguments are given, set defaults and return
            if (args.Length == 0 || args[0] == "/d")
            {

                //Use default start week
                weeks[0] = startWeekConf;

                //check whether selected calendar week is smaller than current calendar week (--> year = current year, else year = prev. year)
                if (weeks[0] <= cal.GetWeekOfYear(now, dfi.CalendarWeekRule, dfi.FirstDayOfWeek))
                {
                    years[0] = now.Year;
                }
                else
                {
                    years[0] = now.Year - 1;
                }

                //Check whether calendar week in that year actually exists:
                lastWeekOfYear = cal.GetWeekOfYear(new DateTime(years[0], 12, 31), dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
                if (weeks[0] > lastWeekOfYear || weeks[0] < 0)
                {
                    argErr = argErr + "Konfigurierte Startwoche des Arbeitsjahres ist fehlerhaft: " + weeks[0] + ".\n";
                    argErr = argErr + "Bitte Konfiguration auf Fehler überprüfen.\n";
                    retVal = 1;
                }

                //End date is Now.
                weeks[1] = cal.GetWeekOfYear(now, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
                years[1] = now.Year;
            }

            //Iterate through all arguments
            for (int i = 0; i < args.Length; i++)
            {
                //Check if too many arguments are given
                if (i >= 2)
                {
                    argErr = argErr + "Zu viele Argumente.\n";
                    retVal = 1;
                    break;
                }

                //Split arguments by dashes
                string[] argTokens = args[i].Split(new char[] { '/' });
                switch (argTokens.Length)
                {
                    //Argument contains no '/': Check whether given value can be identified as a calendar week and set year matchingly.
                    case 1:
                        //Try converting argument to int, assign it to out variable and check whether its in range [1, 52]
                        if (!int.TryParse(argTokens[0], out weeks[i]))
                        {
                            argErr = argErr + "Argument für " + (i == 0 ? "Start" : "End") + "woche ist ungültig: " + args[i] + "\n";
                            retVal = 1;
                        }
                        //check whether selected calendar week is smaller than current calendar week (--> year = current year, else year = prev. year)
                        if (weeks[i] <= cal.GetWeekOfYear(now, dfi.CalendarWeekRule, dfi.FirstDayOfWeek))
                        {
                            years[i] = now.Year;
                        }
                        else
                        {
                            years[i] = now.Year - 1;
                        }
                        break;

                    //Argument contains one '/'
                    case 2:
                        //Try converting week to int, assign it to out variable
                        if (!int.TryParse(argTokens[0], out weeks[i]))
                        {
                            argErr = argErr + "Argument für " + (i == 0 ? "Start" : "End") + "woche ist ungültig: " + args[i] + "\n";
                            retVal = 1;
                        }

                        //Try converting year to int, assign it to out variable
                        if (!(int.TryParse(argTokens[1], out years[i]) && years[i] > 0))
                        {
                            argErr = argErr + "Argument für " + (i == 0 ? "Start" : "End") + "jahr ist ungültig: " + args[i] + "\n";
                            retVal = 1;
                        }
                        //If year has only two digits set it to 20## or 19##
                        if (years[i] < 100)
                        {
                            //If given year is between 0 and last two digits of current year (20## assumed), add 2000, else add 1900.
                            years[i] = (years[i] < (now.Year - 2000)) ? years[i] + 2000 : years[i] + 1900;
                        }
                        break;

                    //Argument contains more than one dash
                    default:
                        argErr = argErr + "Argument " + i + " ist fehlerhaft: " + args[i] + "\n";
                        retVal = 1;
                        break;
                }

            }

            //Set End date to now if not given
            if (weeks[1] == 0 && years[1] == 0)
            {
                weeks[1] = cal.GetWeekOfYear(now, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
                years[1] = now.Year;
            }

            //Check whether calendar week in that year actually exists:
            for (int i = 0; i < 2; i++)
            {
                lastWeekOfYear = cal.GetWeekOfYear(new DateTime(years[i], 12, 31), dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
                if (weeks[i] > lastWeekOfYear || weeks[i] <= 0)
                {
                    argErr = argErr + "Woche ist ausserhalb des gültigen Bereichs: " + weeks[i] + "/" + years[i] + ".\n";
                    retVal = 1;
                }
            }

            //Check whether start date is before end date
            //First check whether end date is bigger than start date, then dates are OK no matter what the week is.
            if (!(years[1] > years[0]))
            {
                //Check if end year is smaller than start year. If that is not the case years are equal. Then check whether end week is smaller than start week
                if (years[1] < years[0] || weeks[1] < weeks[0])
                {
                    argErr = argErr + "Endwoche (KW" + weeks[1] + "/" + years[1] + ") liegt vor Startwoche (KW" + weeks[0] + "/" + years[0] + ").\n";
                    retVal = 1;
                }
            }

            //Generate list of all weeks
            int yearIterator = years[0];
            int weekIterator = weeks[0];

            while (!(yearIterator >= years[1] && weekIterator >= weeks[1]))
            { 
                //Add week to list and add leading 0 if week is smaller than 10
                weekList.Add((weekIterator < 10) ? ("0" + weekIterator + "/" + yearIterator) : (weekIterator + "/" + yearIterator));

                lastWeekOfYear = cal.GetWeekOfYear(new DateTime(yearIterator, 12, 31), dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
                if (weekIterator < lastWeekOfYear)
                {
                    weekIterator++;
                }
                else
                {
                    weekIterator = 1;
                    yearIterator++;
                }
            } 

            //Add last week to list (add leading 0 if week is smaller than 10)
            weekList.Add((weeks[1] < 10) ? ("0" + weeks[1] + "/" + years[1]) : (weeks[1] + "/" + years[1]));

            return retVal;
        }

        private static void PrintHelp()
        {
            string appName = AppDomain.CurrentDomain.FriendlyName;
            Console.WriteLine("<========= HILFE FUER " + appName + " =========>");
            Console.WriteLine("Erlaubte Syntaxen:");
            Console.WriteLine(appName + ":         Startet die Applikation mit der graphischen Benutzeroberfläche.");
            Console.WriteLine(appName + " /d:      Startet die Applikation mit den Standardeinstellungen.");
            Console.WriteLine(appName + " #:       Analyse ab #Startwoche bis aktuelle Woche.");
            Console.WriteLine(appName + " #/#:     Analyse ab #Startwoche/#Jahr bis aktuelle Woche.");
            Console.WriteLine(appName + " # #:     Analyse ab #Startwoche bis #Endwoche.");
            Console.WriteLine(appName + " #/# #/#: Analyse ab #Startwoche/#Jahr bis #Endwoche/#Jahr.");
            Console.WriteLine(appName + " /?:      Gibt diesen Hilfetext aus.");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
        }
    }
}
