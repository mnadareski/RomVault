/******************************************************
 *     ROMVault2 is written by Gordon J.              *
 *     Contact gordon@romvault.com                    *
 *     Copyright 2014                                 *
 ******************************************************/

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ROMVault2.Properties;

namespace ROMVault2
{
    public partial class FrmSettings : Form
    {
        public FrmSettings()
        {
            InitializeComponent();

            cboScanLevel.Items.Clear();
            cboScanLevel.Items.Add("Level1");
            cboScanLevel.Items.Add("Level2");
            cboScanLevel.Items.Add("Level3");

            cboFixLevel.Items.Clear();
            cboFixLevel.Items.Add("TorrentZip Level 1");
            cboFixLevel.Items.Add("TorrentZip Level 2");
            cboFixLevel.Items.Add("TorrentZip Level 3");
            cboFixLevel.Items.Add("Level1");
            cboFixLevel.Items.Add("Level2");
            cboFixLevel.Items.Add("Level3");
        }

        private void FrmConfigLoad(object sender, EventArgs e)
        {
            lblDATRoot.Text = Program.rvSettings.DatRoot;
            cboScanLevel.SelectedIndex = (int)Program.rvSettings.ScanLevel;
            cboFixLevel.SelectedIndex = (int)Program.rvSettings.FixLevel;

            textBox1.Text = "";
            for (int i = 0; i < Program.rvSettings.IgnoreFiles.Count; i++)
                textBox1.Text += Program.rvSettings.IgnoreFiles[i] + Environment.NewLine;

            chkDoubleCheckDelete.Checked = Program.rvSettings.DoubleCheckDelete;
            chkCacheSaveTimer.Checked = Program.rvSettings.CacheSaveTimerEnabled;
            upTime.Value = Program.rvSettings.CacheSaveTimePeriod;
            chkDebugLogs.Checked = Program.rvSettings.DebugLogsEnabled;
        }

        private void BtnCancelClick(object sender, EventArgs e)
        {
            Close();
        }

        private void BtnOkClick(object sender, EventArgs e)
        {
            Program.rvSettings.DatRoot = lblDATRoot.Text;
            Program.rvSettings.ScanLevel = (eScanLevel)cboScanLevel.SelectedIndex;
            Program.rvSettings.FixLevel = (eFixLevel)cboFixLevel.SelectedIndex;

            string strtxt = textBox1.Text;
            strtxt = strtxt.Replace("\r", "");
            string[] strsplit = strtxt.Split('\n');

            Program.rvSettings.IgnoreFiles = new List<string>(strsplit);
            for (int i = 0; i < Program.rvSettings.IgnoreFiles.Count; i++)
            {
                Program.rvSettings.IgnoreFiles[i] = Program.rvSettings.IgnoreFiles[i].Trim();
                if (string.IsNullOrEmpty(Program.rvSettings.IgnoreFiles[i]))
                {
                    Program.rvSettings.IgnoreFiles.RemoveAt(i);
                    i--;
                }
            }

            Program.rvSettings.DoubleCheckDelete = chkDoubleCheckDelete.Checked;
            Program.rvSettings.DebugLogsEnabled = chkDebugLogs.Checked;
            Program.rvSettings.CacheSaveTimerEnabled = chkCacheSaveTimer.Checked;
            Program.rvSettings.CacheSaveTimePeriod = (int)upTime.Value;

            Program.rvSettings.WriteConfig();
            Close();
        }

        private void BtnDatClick(object sender, EventArgs e)
        {
            FolderBrowserDialog browse = new FolderBrowserDialog
                                             {
                                                 ShowNewFolderButton = true,
                                                 Description = Resources.FrmSettings_BtnDatClick_Please_select_a_folder_for_DAT_Root,
                                                 RootFolder = Environment.SpecialFolder.MyComputer,
                                                 SelectedPath = Program.rvSettings.DatRoot
                                             };

            if (browse.ShowDialog() != DialogResult.OK) return;

            lblDATRoot.Text = Utils.RelativePath.MakeRelative(AppDomain.CurrentDomain.BaseDirectory, browse.SelectedPath);
        }

    }
}
