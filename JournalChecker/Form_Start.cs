/*
 * FORM_START.CS
 * JournalChecker module
 * 
 * Module description:
 * -------------------
 * Form_Start.cs contains the following functionalities:
 * - Call form initiliazing function
 * - Fill components with external data
 * - Button listener
 * 
 */

using System;
using System.Windows.Forms;
using UtilitiesCustom;

namespace JournalChecker
{
    public partial class Form_Start : Form
    {

        public string[] Args { private set; get; }

        public Form_Start(int[] weeks, int[] years, string argErr, string confErr)
        {
            InitializeComponent();
            Args = new string[2];

            //Use default values to pre-fill input fields
            InputWeekStart.Text = "" + weeks[0];
            InputYearStart.Text = "" + years[0];
            InputWeekEnd.Text = "" + weeks[1];
            InputYearEnd.Text = "" + years[1];

            //Fill config input fields with current settings
            InputConfDirs.Text = System.Configuration.ConfigurationManager.AppSettings["dirs"];
            InputConfNamelist.Text = System.Configuration.ConfigurationManager.AppSettings["namelist"];
            InputConfTemplate.Text = System.Configuration.ConfigurationManager.AppSettings["report-template"];
            InputConfReports.Text = System.Configuration.ConfigurationManager.AppSettings["reports-location"];
            InputConfFirstWeek.Text = System.Configuration.ConfigurationManager.AppSettings["first-calendar-week"];
            InputConfDistance.Text = System.Configuration.ConfigurationManager.AppSettings["max-levenshtein-distance"];

            //Print errors
            LabelArgErr.Text = argErr;
            LabelConfErr.Text = confErr;
        }

        private void Form_Start_Load(object sender, EventArgs e)
        {

        }

        private void ButtonStart_Click(object sender, EventArgs e)
        {
            Args[0] = InputWeekStart.Text + "/" + InputYearStart.Text;
            Args[1] = InputWeekEnd.Text + "/" + InputYearEnd.Text;

            //Write input to file
            Utilities.UpdateAppSettings("dirs", InputConfDirs.Text);
            Utilities.UpdateAppSettings("namelist", InputConfNamelist.Text);
            Utilities.UpdateAppSettings("report-template", InputConfTemplate.Text);
            Utilities.UpdateAppSettings("reports-location", InputConfReports.Text);
            Utilities.UpdateAppSettings("first-calendar-week", InputConfFirstWeek.Text);
            Utilities.UpdateAppSettings("max-levenshtein-distance", InputConfDistance.Text);

            //Write input to memory
            System.Configuration.ConfigurationManager.AppSettings["dirs"] = InputConfDirs.Text;
            System.Configuration.ConfigurationManager.AppSettings["namelist"] = InputConfNamelist.Text;
            System.Configuration.ConfigurationManager.AppSettings["report-template"] = InputConfTemplate.Text;
            System.Configuration.ConfigurationManager.AppSettings["reports-location"] = InputConfReports.Text;
            System.Configuration.ConfigurationManager.AppSettings["first-calendar-week"] = InputConfFirstWeek.Text;
            System.Configuration.ConfigurationManager.AppSettings["max-levenshtein-distance"] = InputConfDistance.Text;

            this.Close();
        }

        public string[] GetArgs()
        {
            return Args;
        }
    }
}
