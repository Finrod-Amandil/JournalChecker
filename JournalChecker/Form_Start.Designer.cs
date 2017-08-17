/*
 * FORM_START.DESIGNER.CS
 * JournalChecker module
 * 
 * Module description:
 * -------------------
 * Form_Start.Designer.cs contains the following functionalities:
 * - Define form elements
 * 
 */

namespace JournalChecker
{
    partial class Form_Start
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.ButtonStart = new System.Windows.Forms.Button();
            this.LabelSettings = new System.Windows.Forms.Label();
            this.InputWeekStart = new System.Windows.Forms.MaskedTextBox();
            this.InputYearStart = new System.Windows.Forms.MaskedTextBox();
            this.InputWeekEnd = new System.Windows.Forms.MaskedTextBox();
            this.InputYearEnd = new System.Windows.Forms.MaskedTextBox();
            this.LabelStart = new System.Windows.Forms.Label();
            this.LabelEnd = new System.Windows.Forms.Label();
            this.LabelStartWeek = new System.Windows.Forms.Label();
            this.LabelStartYear = new System.Windows.Forms.Label();
            this.LabelEndWeek = new System.Windows.Forms.Label();
            this.LabelEndYear = new System.Windows.Forms.Label();
            this.LabelArgErr = new System.Windows.Forms.Label();
            this.LabelDivider = new System.Windows.Forms.Label();
            this.LabelConfig = new System.Windows.Forms.Label();
            this.LabelConfDirs = new System.Windows.Forms.Label();
            this.InputConfDirs = new System.Windows.Forms.TextBox();
            this.LabelConfNamelist = new System.Windows.Forms.Label();
            this.InputConfNamelist = new System.Windows.Forms.TextBox();
            this.LabelConfTemplate = new System.Windows.Forms.Label();
            this.InputConfTemplate = new System.Windows.Forms.TextBox();
            this.LabelConfReports = new System.Windows.Forms.Label();
            this.InputConfReports = new System.Windows.Forms.TextBox();
            this.LabelConfFirstWeek = new System.Windows.Forms.Label();
            this.InputConfFirstWeek = new System.Windows.Forms.MaskedTextBox();
            this.LabelConfDistance = new System.Windows.Forms.Label();
            this.InputConfDistance = new System.Windows.Forms.TextBox();
            this.LabelConfErr = new System.Windows.Forms.Label();
            this.LabelArgErrCaption = new System.Windows.Forms.Label();
            this.LabelConfErrCaption = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ButtonStart
            // 
            this.ButtonStart.Font = new System.Drawing.Font("Candara", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ButtonStart.Location = new System.Drawing.Point(10, 580);
            this.ButtonStart.Name = "ButtonStart";
            this.ButtonStart.Size = new System.Drawing.Size(450, 40);
            this.ButtonStart.TabIndex = 0;
            this.ButtonStart.Text = "Angaben überprüfen und Journal-Check starten";
            this.ButtonStart.UseVisualStyleBackColor = true;
            this.ButtonStart.Click += new System.EventHandler(this.ButtonStart_Click);
            // 
            // LabelSettings
            // 
            this.LabelSettings.AutoSize = true;
            this.LabelSettings.Font = new System.Drawing.Font("Candara", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelSettings.Location = new System.Drawing.Point(5, 10);
            this.LabelSettings.Name = "LabelSettings";
            this.LabelSettings.Size = new System.Drawing.Size(152, 29);
            this.LabelSettings.TabIndex = 1;
            this.LabelSettings.Text = "Einstellungen";
            // 
            // InputWeekStart
            // 
            this.InputWeekStart.Location = new System.Drawing.Point(160, 75);
            this.InputWeekStart.Mask = "00";
            this.InputWeekStart.Name = "InputWeekStart";
            this.InputWeekStart.Size = new System.Drawing.Size(33, 20);
            this.InputWeekStart.TabIndex = 2;
            // 
            // InputYearStart
            // 
            this.InputYearStart.Location = new System.Drawing.Point(200, 75);
            this.InputYearStart.Mask = "0000";
            this.InputYearStart.Name = "InputYearStart";
            this.InputYearStart.Size = new System.Drawing.Size(50, 20);
            this.InputYearStart.TabIndex = 3;
            // 
            // InputWeekEnd
            // 
            this.InputWeekEnd.Location = new System.Drawing.Point(160, 125);
            this.InputWeekEnd.Mask = "00";
            this.InputWeekEnd.Name = "InputWeekEnd";
            this.InputWeekEnd.Size = new System.Drawing.Size(33, 20);
            this.InputWeekEnd.TabIndex = 4;
            // 
            // InputYearEnd
            // 
            this.InputYearEnd.Location = new System.Drawing.Point(200, 125);
            this.InputYearEnd.Mask = "0000";
            this.InputYearEnd.Name = "InputYearEnd";
            this.InputYearEnd.Size = new System.Drawing.Size(50, 20);
            this.InputYearEnd.TabIndex = 5;
            // 
            // LabelStart
            // 
            this.LabelStart.AutoSize = true;
            this.LabelStart.Location = new System.Drawing.Point(10, 80);
            this.LabelStart.Name = "LabelStart";
            this.LabelStart.Size = new System.Drawing.Size(131, 13);
            this.LabelStart.TabIndex = 6;
            this.LabelStart.Text = "Erste zu prüfende Woche:";
            // 
            // LabelEnd
            // 
            this.LabelEnd.AutoSize = true;
            this.LabelEnd.Location = new System.Drawing.Point(10, 130);
            this.LabelEnd.Name = "LabelEnd";
            this.LabelEnd.Size = new System.Drawing.Size(136, 13);
            this.LabelEnd.TabIndex = 7;
            this.LabelEnd.Text = "Letzte zu prüfende Woche:";
            // 
            // LabelStartWeek
            // 
            this.LabelStartWeek.AutoSize = true;
            this.LabelStartWeek.Location = new System.Drawing.Point(157, 60);
            this.LabelStartWeek.Name = "LabelStartWeek";
            this.LabelStartWeek.Size = new System.Drawing.Size(25, 13);
            this.LabelStartWeek.TabIndex = 8;
            this.LabelStartWeek.Text = "KW";
            // 
            // LabelStartYear
            // 
            this.LabelStartYear.AutoSize = true;
            this.LabelStartYear.Location = new System.Drawing.Point(200, 60);
            this.LabelStartYear.Name = "LabelStartYear";
            this.LabelStartYear.Size = new System.Drawing.Size(27, 13);
            this.LabelStartYear.TabIndex = 9;
            this.LabelStartYear.Text = "Jahr";
            // 
            // LabelEndWeek
            // 
            this.LabelEndWeek.AutoSize = true;
            this.LabelEndWeek.Location = new System.Drawing.Point(157, 110);
            this.LabelEndWeek.Name = "LabelEndWeek";
            this.LabelEndWeek.Size = new System.Drawing.Size(25, 13);
            this.LabelEndWeek.TabIndex = 10;
            this.LabelEndWeek.Text = "KW";
            // 
            // LabelEndYear
            // 
            this.LabelEndYear.AutoSize = true;
            this.LabelEndYear.Location = new System.Drawing.Point(200, 110);
            this.LabelEndYear.Name = "LabelEndYear";
            this.LabelEndYear.Size = new System.Drawing.Size(27, 13);
            this.LabelEndYear.TabIndex = 11;
            this.LabelEndYear.Text = "Jahr";
            // 
            // LabelArgErr
            // 
            this.LabelArgErr.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.LabelArgErr.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LabelArgErr.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelArgErr.ForeColor = System.Drawing.Color.Red;
            this.LabelArgErr.Location = new System.Drawing.Point(10, 190);
            this.LabelArgErr.Name = "LabelArgErr";
            this.LabelArgErr.Size = new System.Drawing.Size(450, 50);
            this.LabelArgErr.TabIndex = 12;
            // 
            // LabelDivider
            // 
            this.LabelDivider.Location = new System.Drawing.Point(-5, 240);
            this.LabelDivider.Margin = new System.Windows.Forms.Padding(0);
            this.LabelDivider.Name = "LabelDivider";
            this.LabelDivider.Size = new System.Drawing.Size(485, 15);
            this.LabelDivider.TabIndex = 13;
            this.LabelDivider.Text = "_________________________________________________________________________________" +
    "_";
            // 
            // LabelConfig
            // 
            this.LabelConfig.AutoSize = true;
            this.LabelConfig.Font = new System.Drawing.Font("Candara", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelConfig.Location = new System.Drawing.Point(5, 260);
            this.LabelConfig.Name = "LabelConfig";
            this.LabelConfig.Size = new System.Drawing.Size(155, 29);
            this.LabelConfig.TabIndex = 14;
            this.LabelConfig.Text = "Konfiguration";
            // 
            // LabelConfDirs
            // 
            this.LabelConfDirs.AutoSize = true;
            this.LabelConfDirs.Location = new System.Drawing.Point(10, 310);
            this.LabelConfDirs.Name = "LabelConfDirs";
            this.LabelConfDirs.Size = new System.Drawing.Size(136, 13);
            this.LabelConfDirs.TabIndex = 15;
            this.LabelConfDirs.Text = "Zu prüfende Verzeichnisse:";
            // 
            // InputConfDirs
            // 
            this.InputConfDirs.Location = new System.Drawing.Point(160, 305);
            this.InputConfDirs.Name = "InputConfDirs";
            this.InputConfDirs.Size = new System.Drawing.Size(300, 20);
            this.InputConfDirs.TabIndex = 16;
            // 
            // LabelConfNamelist
            // 
            this.LabelConfNamelist.AutoSize = true;
            this.LabelConfNamelist.Location = new System.Drawing.Point(10, 340);
            this.LabelConfNamelist.Name = "LabelConfNamelist";
            this.LabelConfNamelist.Size = new System.Drawing.Size(106, 13);
            this.LabelConfNamelist.TabIndex = 17;
            this.LabelConfNamelist.Text = "Pfad zu Namensliste:";
            // 
            // InputConfNamelist
            // 
            this.InputConfNamelist.Location = new System.Drawing.Point(160, 335);
            this.InputConfNamelist.Name = "InputConfNamelist";
            this.InputConfNamelist.Size = new System.Drawing.Size(300, 20);
            this.InputConfNamelist.TabIndex = 18;
            // 
            // LabelConfTemplate
            // 
            this.LabelConfTemplate.AutoSize = true;
            this.LabelConfTemplate.Location = new System.Drawing.Point(10, 370);
            this.LabelConfTemplate.Name = "LabelConfTemplate";
            this.LabelConfTemplate.Size = new System.Drawing.Size(126, 13);
            this.LabelConfTemplate.TabIndex = 19;
            this.LabelConfTemplate.Text = "Pfad zu HTML-Template:";
            // 
            // InputConfTemplate
            // 
            this.InputConfTemplate.Location = new System.Drawing.Point(160, 365);
            this.InputConfTemplate.Name = "InputConfTemplate";
            this.InputConfTemplate.Size = new System.Drawing.Size(300, 20);
            this.InputConfTemplate.TabIndex = 20;
            // 
            // LabelConfReports
            // 
            this.LabelConfReports.AutoSize = true;
            this.LabelConfReports.Location = new System.Drawing.Point(10, 400);
            this.LabelConfReports.Name = "LabelConfReports";
            this.LabelConfReports.Size = new System.Drawing.Size(120, 13);
            this.LabelConfReports.TabIndex = 21;
            this.LabelConfReports.Text = "Speicherort für Reporte:";
            // 
            // InputConfReports
            // 
            this.InputConfReports.Location = new System.Drawing.Point(160, 395);
            this.InputConfReports.Name = "InputConfReports";
            this.InputConfReports.Size = new System.Drawing.Size(300, 20);
            this.InputConfReports.TabIndex = 22;
            // 
            // LabelConfFirstWeek
            // 
            this.LabelConfFirstWeek.AutoSize = true;
            this.LabelConfFirstWeek.Location = new System.Drawing.Point(10, 430);
            this.LabelConfFirstWeek.Name = "LabelConfFirstWeek";
            this.LabelConfFirstWeek.Size = new System.Drawing.Size(240, 13);
            this.LabelConfFirstWeek.TabIndex = 23;
            this.LabelConfFirstWeek.Text = "Erste Kalenderwoche des aktuellen Arbeitsjahres:";
            // 
            // InputConfFirstWeek
            // 
            this.InputConfFirstWeek.Location = new System.Drawing.Point(260, 425);
            this.InputConfFirstWeek.Mask = "00";
            this.InputConfFirstWeek.Name = "InputConfFirstWeek";
            this.InputConfFirstWeek.Size = new System.Drawing.Size(33, 20);
            this.InputConfFirstWeek.TabIndex = 24;
            // 
            // LabelConfDistance
            // 
            this.LabelConfDistance.AutoSize = true;
            this.LabelConfDistance.Location = new System.Drawing.Point(10, 460);
            this.LabelConfDistance.Name = "LabelConfDistance";
            this.LabelConfDistance.Size = new System.Drawing.Size(193, 13);
            this.LabelConfDistance.TabIndex = 25;
            this.LabelConfDistance.Text = "Maximal zulässige Levenshtein-Distanz:";
            // 
            // InputConfDistance
            // 
            this.InputConfDistance.Location = new System.Drawing.Point(260, 455);
            this.InputConfDistance.Name = "InputConfDistance";
            this.InputConfDistance.Size = new System.Drawing.Size(33, 20);
            this.InputConfDistance.TabIndex = 26;
            // 
            // LabelConfErr
            // 
            this.LabelConfErr.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.LabelConfErr.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LabelConfErr.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelConfErr.ForeColor = System.Drawing.Color.Red;
            this.LabelConfErr.Location = new System.Drawing.Point(10, 520);
            this.LabelConfErr.Name = "LabelConfErr";
            this.LabelConfErr.Size = new System.Drawing.Size(450, 50);
            this.LabelConfErr.TabIndex = 27;
            // 
            // LabelArgErrCaption
            // 
            this.LabelArgErrCaption.AutoSize = true;
            this.LabelArgErrCaption.Location = new System.Drawing.Point(10, 170);
            this.LabelArgErrCaption.Name = "LabelArgErrCaption";
            this.LabelArgErrCaption.Size = new System.Drawing.Size(150, 13);
            this.LabelArgErrCaption.TabIndex = 28;
            this.LabelArgErrCaption.Text = "Einstellungs-Fehlermeldungen:";
            // 
            // LabelConfErrCaption
            // 
            this.LabelConfErrCaption.AutoSize = true;
            this.LabelConfErrCaption.Location = new System.Drawing.Point(10, 500);
            this.LabelConfErrCaption.Name = "LabelConfErrCaption";
            this.LabelConfErrCaption.Size = new System.Drawing.Size(161, 13);
            this.LabelConfErrCaption.TabIndex = 29;
            this.LabelConfErrCaption.Text = "Konfigurations-Fehlermeldungen:";
            // 
            // Form_Start
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(469, 631);
            this.Controls.Add(this.LabelConfErrCaption);
            this.Controls.Add(this.LabelArgErrCaption);
            this.Controls.Add(this.LabelConfErr);
            this.Controls.Add(this.InputConfDistance);
            this.Controls.Add(this.LabelConfDistance);
            this.Controls.Add(this.InputConfFirstWeek);
            this.Controls.Add(this.LabelConfFirstWeek);
            this.Controls.Add(this.InputConfReports);
            this.Controls.Add(this.LabelConfReports);
            this.Controls.Add(this.InputConfTemplate);
            this.Controls.Add(this.LabelConfTemplate);
            this.Controls.Add(this.InputConfNamelist);
            this.Controls.Add(this.LabelConfNamelist);
            this.Controls.Add(this.InputConfDirs);
            this.Controls.Add(this.LabelConfDirs);
            this.Controls.Add(this.LabelConfig);
            this.Controls.Add(this.LabelDivider);
            this.Controls.Add(this.LabelArgErr);
            this.Controls.Add(this.LabelEndYear);
            this.Controls.Add(this.LabelEndWeek);
            this.Controls.Add(this.LabelStartYear);
            this.Controls.Add(this.LabelStartWeek);
            this.Controls.Add(this.LabelEnd);
            this.Controls.Add(this.LabelStart);
            this.Controls.Add(this.InputYearEnd);
            this.Controls.Add(this.InputWeekEnd);
            this.Controls.Add(this.InputYearStart);
            this.Controls.Add(this.InputWeekStart);
            this.Controls.Add(this.LabelSettings);
            this.Controls.Add(this.ButtonStart);
            this.Name = "Form_Start";
            this.Text = "Journal Checker";
            this.Load += new System.EventHandler(this.Form_Start_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button ButtonStart;
        private System.Windows.Forms.Label LabelSettings;
        private System.Windows.Forms.MaskedTextBox InputWeekStart;
        private System.Windows.Forms.MaskedTextBox InputYearStart;
        private System.Windows.Forms.MaskedTextBox InputWeekEnd;
        private System.Windows.Forms.MaskedTextBox InputYearEnd;
        private System.Windows.Forms.Label LabelStart;
        private System.Windows.Forms.Label LabelEnd;
        private System.Windows.Forms.Label LabelStartWeek;
        private System.Windows.Forms.Label LabelStartYear;
        private System.Windows.Forms.Label LabelEndWeek;
        private System.Windows.Forms.Label LabelEndYear;
        private System.Windows.Forms.Label LabelArgErr;
        private System.Windows.Forms.Label LabelDivider;
        private System.Windows.Forms.Label LabelConfig;
        private System.Windows.Forms.Label LabelConfDirs;
        private System.Windows.Forms.TextBox InputConfDirs;
        private System.Windows.Forms.Label LabelConfNamelist;
        private System.Windows.Forms.TextBox InputConfNamelist;
        private System.Windows.Forms.Label LabelConfTemplate;
        private System.Windows.Forms.TextBox InputConfTemplate;
        private System.Windows.Forms.Label LabelConfReports;
        private System.Windows.Forms.TextBox InputConfReports;
        private System.Windows.Forms.Label LabelConfFirstWeek;
        private System.Windows.Forms.MaskedTextBox InputConfFirstWeek;
        private System.Windows.Forms.Label LabelConfDistance;
        private System.Windows.Forms.TextBox InputConfDistance;
        private System.Windows.Forms.Label LabelConfErr;
        private System.Windows.Forms.Label LabelArgErrCaption;
        private System.Windows.Forms.Label LabelConfErrCaption;
    }
}

