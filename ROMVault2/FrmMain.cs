/******************************************************
 *     ROMVault2 is written by Gordon J.              *
 *     Contact gordon@romvault.com                    *
 *     Copyright 2010                                 *
 ******************************************************/
using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.ServiceModel;
using System.Windows.Forms;
using ROMVault2.Properties;
using ROMVault2.RvDB;
using ROMVault2.Utils;
using ROMVault2.SupportedFiles;

namespace ROMVault2
{
    public partial class FrmMain : Form
    {

        private static readonly Color CBlue = Color.FromArgb(214, 214, 255);
        private static readonly Color CGreyBlue = Color.FromArgb(214, 224, 255);
        private static readonly Color CRed = Color.FromArgb(255, 214, 214);
        private static readonly Color CBrightRed = Color.FromArgb(255, 0, 0);
        private static readonly Color CGreen = Color.FromArgb(214, 255, 214);
        private static readonly Color CGrey = Color.FromArgb(214, 214, 214);
        private static readonly Color CCyan = Color.FromArgb(214, 255, 255);
        private static readonly Color CCyanGrey = Color.FromArgb(214, 225, 225);
        private static readonly Color CMagenta = Color.FromArgb(255, 214, 255);
        private static readonly Color CBrown = Color.FromArgb(140, 80, 80);
        private static readonly Color CPurple = Color.FromArgb(214, 140, 214);
        private static readonly Color CYellow = Color.FromArgb(255, 255, 214);
        private static readonly Color COrange = Color.FromArgb(255, 214, 140);
        private static readonly Color CWhite = Color.FromArgb(255, 255, 255);

        private readonly Color[] _displayColor;

        private bool _updatingGameGrid;
	
        private int GameGridSortColumnIndex = 0;
        private SortOrder GameGridSortOrder = SortOrder.Descending;

        public static int[] GameGridColumnXPositions;

        private FrmKey _fk;

        private Single _scaleFactorX = 1;
        private Single _scaleFactorY = 1;

        protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
        {
            base.ScaleControl(factor, specified);
            splitContainer1.SplitterDistance = (int)(splitContainer1.SplitterDistance * factor.Width);
            splitContainer2.SplitterDistance = (int)(splitContainer2.SplitterDistance * factor.Width);
            splitContainer2.Panel1MinSize = (int)(splitContainer2.Panel1MinSize * factor.Width);

            splitContainer3.SplitterDistance = (int)(splitContainer3.SplitterDistance * factor.Height);
            splitContainer4.SplitterDistance = (int)(splitContainer4.SplitterDistance * factor.Height);

            _scaleFactorX *= factor.Width;
            _scaleFactorY *= factor.Height;
        }

        public FrmMain()
        {

            InitializeComponent();
            addGameGrid();
            Text = string.Format("RomVault ({0}.{1})  {2}", Program.Version, Program.SubVersion, Application.StartupPath);




            _displayColor = new Color[(int)RepStatus.EndValue];

            // RepStatus.UnSet

            _displayColor[(int)RepStatus.UnScanned] = CBlue;

            _displayColor[(int)RepStatus.DirCorrect] = CGreen;
            _displayColor[(int)RepStatus.DirMissing] = CRed;
            _displayColor[(int)RepStatus.DirCorrupt] = CBrightRed;  //BrightRed

            _displayColor[(int)RepStatus.Missing] = CRed;
            _displayColor[(int)RepStatus.Correct] = CGreen;
            _displayColor[(int)RepStatus.NotCollected] = CGrey;
            _displayColor[(int)RepStatus.UnNeeded] = CCyanGrey;
            _displayColor[(int)RepStatus.Unknown] = CCyan;
            _displayColor[(int)RepStatus.InToSort] = CMagenta;

            _displayColor[(int)RepStatus.Corrupt] = CBrightRed;  //BrightRed
            _displayColor[(int)RepStatus.Ignore] = CGreyBlue;

            _displayColor[(int)RepStatus.CanBeFixed] = CYellow;
            _displayColor[(int)RepStatus.MoveToSort] = CPurple;
            _displayColor[(int)RepStatus.Delete] = CBrown;
            _displayColor[(int)RepStatus.NeededForFix] = COrange;
            _displayColor[(int)RepStatus.Rename] = COrange;

            _displayColor[(int)RepStatus.CorruptCanBeFixed] = CYellow;
            _displayColor[(int)RepStatus.MoveToCorrupt] = CPurple;       //Missing


            _displayColor[(int)RepStatus.Deleted] = CWhite;

            GameGridColumnXPositions = new int[(int)RepStatus.EndValue];

            DirTree.Setup(ref DB.DirTree);

            splitContainer3_Panel1_Resize(new object(), new EventArgs());
            splitContainer4_Panel1_Resize(new object(), new EventArgs());
        }

        private Label lblSIName;
        private Label lblSITName;

        private Label lblSIDescription;
        private Label lblSITDescription;

        private Label lblSIManufacturer;
        private Label lblSITManufacturer;

        private Label lblSICloneOf;
        private Label lblSITCloneOf;

        private Label lblSIRomOf;
        private Label lblSITRomOf;

        private Label lblSIYear;
        private Label lblSITYear;

        private Label lblSITotalRoms;
        private Label lblSITTotalRoms;

        //Trurip Extra Data
        private Label lblSIPublisher;
        private Label lblSITPublisher;

        private Label lblSIDeveloper;
        private Label lblSITDeveloper;

        private Label lblSIEdition;
        private Label lblSITEdition;

        private Label lblSIVersion;
        private Label lblSITVersion;

        private Label lblSIType;
        private Label lblSITType;

        private Label lblSIMedia;
        private Label lblSITMedia;

        private Label lblSILanguage;
        private Label lblSITLanguage;

        private Label lblSIPlayers;
        private Label lblSITPlayers;

        private Label lblSIRatings;
        private Label lblSITRatings;

        private Label lblSIGenre;
        private Label lblSITGenre;

        private Label lblSIPeripheral;
        private Label lblSITPeripheral;

        private Label lblSIBarCode;
        private Label lblSITBarCode;

        private Label lblSIMediaCatalogNumber;
        private Label lblSITMediaCatalogNumber;

        private void addGameGrid()
        {
            lblSIName = new Label { Location = SPoint(6, 15), Size = SSize(76, 13), Text = "Name :", TextAlign = ContentAlignment.TopRight };
            lblSITName = new Label { Location = SPoint(84, 14), Size = SSize(320, 17), BorderStyle = BorderStyle.FixedSingle };
            gbSetInfo.Controls.Add(lblSIName);
            gbSetInfo.Controls.Add(lblSITName);

            lblSIDescription = new Label { Location = SPoint(6, 31), Size = SSize(76, 13), Text = "Description :", TextAlign = ContentAlignment.TopRight, Visible = false };
            lblSITDescription = new Label { Location = SPoint(84, 30), Size = SSize(320, 17), BorderStyle = BorderStyle.FixedSingle, Visible = false };
            gbSetInfo.Controls.Add(lblSIDescription);
            gbSetInfo.Controls.Add(lblSITDescription);

            lblSIManufacturer = new Label { Location = SPoint(6, 47), Size = SSize(76, 13), Text = "Manufacturer :", TextAlign = ContentAlignment.TopRight, Visible = false };
            lblSITManufacturer = new Label { Location = SPoint(84, 46), Size = SSize(320, 17), BorderStyle = BorderStyle.FixedSingle, Visible = false };
            gbSetInfo.Controls.Add(lblSIManufacturer);
            gbSetInfo.Controls.Add(lblSITManufacturer);

            lblSICloneOf = new Label { Location = SPoint(6, 63), Size = SSize(76, 13), Text = "Clone of :", TextAlign = ContentAlignment.TopRight, Visible = false };
            lblSITCloneOf = new Label { Location = SPoint(84, 62), Size = SSize(120, 17), BorderStyle = BorderStyle.FixedSingle, Visible = false };
            gbSetInfo.Controls.Add(lblSICloneOf);
            gbSetInfo.Controls.Add(lblSITCloneOf);

            lblSIYear = new Label { Location = SPoint(206, 63), Size = SSize(76, 13), Text = "Year :", TextAlign = ContentAlignment.TopRight, Visible = false };
            lblSITYear = new Label { Location = SPoint(284, 62), Size = SSize(120, 17), BorderStyle = BorderStyle.FixedSingle, Visible = false };
            gbSetInfo.Controls.Add(lblSIYear);
            gbSetInfo.Controls.Add(lblSITYear);


            lblSIRomOf = new Label { Location = SPoint(6, 79), Size = SSize(76, 13), Text = "ROM of :", TextAlign = ContentAlignment.TopRight, Visible = false };
            lblSITRomOf = new Label { Location = SPoint(84, 78), Size = SSize(120, 17), BorderStyle = BorderStyle.FixedSingle, Visible = false };
            gbSetInfo.Controls.Add(lblSIRomOf);
            gbSetInfo.Controls.Add(lblSITRomOf);

            lblSITotalRoms = new Label { Location = SPoint(206, 79), Size = SSize(76, 13), Text = "Total ROMs :", TextAlign = ContentAlignment.TopRight, Visible = false };
            lblSITTotalRoms = new Label { Location = SPoint(284, 78), Size = SSize(120, 17), BorderStyle = BorderStyle.FixedSingle, Visible = false };
            gbSetInfo.Controls.Add(lblSITotalRoms);
            gbSetInfo.Controls.Add(lblSITTotalRoms);

            //Trurip

            lblSIPublisher = new Label { Location = SPoint(6, 47), Size = SSize(76, 13), Text = "Publisher :", TextAlign = ContentAlignment.TopRight, Visible = false };
            lblSITPublisher = new Label { Location = SPoint(84, 46), Size = SSize(320, 17), BorderStyle = BorderStyle.FixedSingle, Visible = false };
            gbSetInfo.Controls.Add(lblSIPublisher);
            gbSetInfo.Controls.Add(lblSITPublisher);

            lblSIDeveloper = new Label { Location = SPoint(6, 63), Size = SSize(76, 13), Text = "Developer :", TextAlign = ContentAlignment.TopRight, Visible = false };
            lblSITDeveloper = new Label { Location = SPoint(84, 62), Size = SSize(320, 17), BorderStyle = BorderStyle.FixedSingle, Visible = false };
            gbSetInfo.Controls.Add(lblSIDeveloper);
            gbSetInfo.Controls.Add(lblSITDeveloper);



            lblSIEdition = new Label { Location = SPoint(6, 79), Size = SSize(76, 13), Text = "Edition :", TextAlign = ContentAlignment.TopRight, Visible = false };
            lblSITEdition = new Label { Location = SPoint(84, 78), Size = SSize(120, 17), BorderStyle = BorderStyle.FixedSingle, Visible = false };
            gbSetInfo.Controls.Add(lblSIEdition);
            gbSetInfo.Controls.Add(lblSITEdition);

            lblSIVersion = new Label { Location = SPoint(206, 79), Size = SSize(76, 13), Text = "Version :", TextAlign = ContentAlignment.TopRight, Visible = false };
            lblSITVersion = new Label { Location = SPoint(284, 78), Size = SSize(120, 17), BorderStyle = BorderStyle.FixedSingle, Visible = false };
            gbSetInfo.Controls.Add(lblSIVersion);
            gbSetInfo.Controls.Add(lblSITVersion);

            lblSIType = new Label { Location = SPoint(406, 79), Size = SSize(76, 13), Text = "Type :", TextAlign = ContentAlignment.TopRight, Visible = false };
            lblSITType = new Label { Location = SPoint(484, 78), Size = SSize(120, 17), BorderStyle = BorderStyle.FixedSingle, Visible = false };
            gbSetInfo.Controls.Add(lblSIType);
            gbSetInfo.Controls.Add(lblSITType);


            lblSIMedia = new Label { Location = SPoint(6, 95), Size = SSize(76, 13), Text = "Media :", TextAlign = ContentAlignment.TopRight, Visible = false };
            lblSITMedia = new Label { Location = SPoint(84, 94), Size = SSize(120, 17), BorderStyle = BorderStyle.FixedSingle, Visible = false };
            gbSetInfo.Controls.Add(lblSIMedia);
            gbSetInfo.Controls.Add(lblSITMedia);

            lblSILanguage = new Label { Location = SPoint(206, 95), Size = SSize(76, 13), Text = "Language :", TextAlign = ContentAlignment.TopRight, Visible = false };
            lblSITLanguage = new Label { Location = SPoint(284, 94), Size = SSize(120, 17), BorderStyle = BorderStyle.FixedSingle, Visible = false };
            gbSetInfo.Controls.Add(lblSILanguage);
            gbSetInfo.Controls.Add(lblSITLanguage);

            lblSIPlayers = new Label { Location = SPoint(406, 95), Size = SSize(76, 13), Text = "Players :", TextAlign = ContentAlignment.TopRight, Visible = false };
            lblSITPlayers = new Label { Location = SPoint(484, 94), Size = SSize(120, 17), BorderStyle = BorderStyle.FixedSingle, Visible = false };
            gbSetInfo.Controls.Add(lblSIPlayers);
            gbSetInfo.Controls.Add(lblSITPlayers);



            lblSIRatings = new Label { Location = SPoint(6, 111), Size = SSize(76, 13), Text = "Ratings :", TextAlign = ContentAlignment.TopRight, Visible = false };
            lblSITRatings = new Label { Location = SPoint(84, 110), Size = SSize(120, 17), BorderStyle = BorderStyle.FixedSingle, Visible = false };
            gbSetInfo.Controls.Add(lblSIRatings);
            gbSetInfo.Controls.Add(lblSITRatings);

            lblSIGenre = new Label { Location = SPoint(206, 111), Size = SSize(76, 13), Text = "Genre :", TextAlign = ContentAlignment.TopRight, Visible = false };
            lblSITGenre = new Label { Location = SPoint(284, 110), Size = SSize(120, 17), BorderStyle = BorderStyle.FixedSingle, Visible = false };
            gbSetInfo.Controls.Add(lblSIGenre);
            gbSetInfo.Controls.Add(lblSITGenre);

            lblSIPeripheral  = new Label { Location = SPoint(406, 111), Size = SSize(76, 13), Text = "Peripheral :", TextAlign = ContentAlignment.TopRight, Visible = false };
            lblSITPeripheral = new Label { Location = SPoint(484, 110), Size = SSize(120, 17), BorderStyle = BorderStyle.FixedSingle, Visible = false };
            gbSetInfo.Controls.Add(lblSIPeripheral);
            gbSetInfo.Controls.Add(lblSITPeripheral);


            lblSIBarCode = new Label { Location = SPoint(6, 127), Size = SSize(76, 13), Text = "Barcode :", TextAlign = ContentAlignment.TopRight, Visible = false };
            lblSITBarCode = new Label { Location = SPoint(84, 126), Size = SSize(120, 17), BorderStyle = BorderStyle.FixedSingle, Visible = false };
            gbSetInfo.Controls.Add(lblSIBarCode);
            gbSetInfo.Controls.Add(lblSITBarCode);

            lblSIMediaCatalogNumber = new Label { Location = SPoint(406, 127), Size = SSize(76, 13), Text = "Cat. No. :", TextAlign = ContentAlignment.TopRight, Visible = false };
            lblSITMediaCatalogNumber = new Label { Location = SPoint(484, 126), Size = SSize(120, 17), BorderStyle = BorderStyle.FixedSingle, Visible = false };
            gbSetInfo.Controls.Add(lblSIMediaCatalogNumber);
            gbSetInfo.Controls.Add(lblSITMediaCatalogNumber);

        }

        private Point SPoint(int x, int y)
        {
            return new Point((int)(x*_scaleFactorX),(int)(y*_scaleFactorY));
        }
        private Size SSize(int x, int y)
        {
            return new Size((int)(x * _scaleFactorX), (int)(y * _scaleFactorY));
        }


        public override sealed string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        #region "Main Buttons"

        private void TsmUpdateDaTsClick(object sender, EventArgs e)
        {
            UpdateDats();
        }
        private void BtnUpdateDatsClick(object sender, EventArgs e)
        {
            UpdateDats();
        }

        private void UpdateDats()
        {

            FrmProgressWindow progress = new FrmProgressWindow(this, "Scanning Dats", DatUpdate.UpdateDat);
            progress.ShowDialog(this);
            progress.Dispose();

            DirTree.Setup(ref DB.DirTree);
            DatSetSelected(DirTree.Selected);
        }




        private void TsmScanLevel1Click(object sender, EventArgs e)
        {
            ScanRoms(eScanLevel.Level1);
        }
        private void TsmScanLevel2Click(object sender, EventArgs e)
        {
            ScanRoms(eScanLevel.Level2);
        }
        private void TsmScanLevel3Click(object sender, EventArgs e)
        {
            ScanRoms(eScanLevel.Level3);
        }
        private void BtnScanRomsClick(object sender, EventArgs e)
        {
            ScanRoms(Settings.ScanLevel);
        }

        private void ScanRoms(eScanLevel sd)
        {
            FileScanning.EScanLevel = sd;
            FrmProgressWindow progress = new FrmProgressWindow(this, "Scanning Dirs", FileScanning.ScanFiles);
            progress.ShowDialog(this);
            progress.Dispose();

            DatSetSelected(DirTree.Selected);
        }


        private void TsmFindFixesClick(object sender, EventArgs e)
        {
            FindFix();
        }
        private void BtnFindFixesClick(object sender, EventArgs e)
        {
            FindFix();
        }

        private void FindFix()
        {
#if NEWFINDFIX
            FrmProgressWindow progress = new FrmProgressWindow(this, "Finding Fixes", FindFixesNew.ScanFiles);
            progress.ShowDialog(this);
            progress.Dispose();

            DatSetSelected(DirTree.Selected);
#else
            FrmProgressWindow progress = new FrmProgressWindow(this, "Finding Fixes", FindFixes.ScanFiles);
            progress.ShowDialog(this);
            progress.Dispose();

            DatSetSelected(DirTree.Selected);
#endif
        }




        private void BtnFixFilesClick(object sender, EventArgs e)
        {
            FixFiles();
        }
        private void FixFilesToolStripMenuItemClick(object sender, EventArgs e)
        {
            FixFiles();
        }


        private void FixFiles()
        {
            FrmProgressWindowFix progress = new FrmProgressWindowFix(this);
            progress.ShowDialog(this);
            progress.Dispose();

            DatSetSelected(DirTree.Selected);
        }



        #endregion

        private void DirTreeRvSelected(object sender, MouseEventArgs e)
        {
            RvDir cf = (RvDir)sender;
            if (cf != DirTree.GetSelected())
                DatSetSelected(cf);

            if (e.Button == MouseButtons.Right)
            {
                RvDir tn = (RvDir)sender;

                ContextMenu mnuContext = new ContextMenu();

                MenuItem mnuFile = new MenuItem
                {
                    Index = 0,
                    Text = Resources.FrmMain_DirTreeRvSelected_Set_ROM_DIR,
                    Tag = tn.TreeFullName
                };
                mnuFile.Click += MnuFileClick;
                mnuContext.MenuItems.Add(mnuFile);

                MenuItem mnuMakeDat = new MenuItem
                {
                    Index = 1,
                    Text = @"Make Dat",
                    Tag = tn
                };
                mnuMakeDat.Click += MnuMakeDatClick;
                mnuContext.MenuItems.Add(mnuMakeDat);


                mnuContext.Show(DirTree, e.Location);
            }
        }

        #region "DAT display code"

        private void DatSetSelected(RvBase cf)
        {
            DirTree.Refresh();

            if (Settings.IsMono)
            {
                if (GameGrid.RowCount > 0)
                    GameGrid.CurrentCell = GameGrid[0,0];
                if (RomGrid.RowCount > 0)
                    RomGrid.CurrentCell = RomGrid[0,0];
            }

            GameGrid.Rows.Clear();
            RomGrid.Rows.Clear();

            // clear sorting
            GameGrid.Columns[GameGridSortColumnIndex].HeaderCell.SortGlyphDirection = SortOrder.None;
            GameGridSortColumnIndex = 0;
            GameGridSortOrder = SortOrder.Descending;

            if (cf == null)
                return;

            UpdateGameGrid((RvDir)cf);


        }


        private static void MnuFileClick(object sender, EventArgs e)
        {
            FrmSetDir sd = new FrmSetDir();

            MenuItem mi = (MenuItem)sender;
            string tDir = (string)mi.Tag;
            Debug.WriteLine("Opening Dir Options for " + tDir);

            sd.SetLocation(tDir);
            sd.ShowDialog();

            sd.Dispose();
        }

        private static void MnuMakeDatClick(object sender, EventArgs e)
        {
            MenuItem mi = (MenuItem)sender;
            RvDir thisDir = (RvDir) mi.Tag;
            DatMaker.MakeDatFromDir(thisDir);
        }

        private void splitContainer3_Panel1_Resize(object sender, EventArgs e)
        {
            // fixes a rendering issue in mono
            if (splitContainer3.Panel1.Width == 0) return;

            gbDatInfo.Width = splitContainer3.Panel1.Width - (gbDatInfo.Left * 2);
        }

        private void gbDatInfo_Resize(object sender, EventArgs e)
        {
            const int leftPos = 89;
            int rightPos = (int)(gbDatInfo.Width / _scaleFactorX) - 15;
            if (rightPos > 600) rightPos = 600;
            int width = rightPos - leftPos;
            int widthB1 = (int)((double)width * 120 / 340);
            int leftB2 = rightPos - widthB1;


            int backD = 97;

            width = (int)(width * _scaleFactorX);
            widthB1 = (int)(widthB1 * _scaleFactorX);
            leftB2 = (int)(leftB2 * _scaleFactorX);
            backD = (int)(backD * _scaleFactorX);


            lblDITName.Width = width;
            lblDITDescription.Width = width;

            lblDITCategory.Width = widthB1;
            lblDITAuthor.Width = widthB1;

            lblDIVersion.Left = leftB2 - backD;
            lblDIDate.Left = leftB2 - backD;

            lblDITVersion.Left = leftB2;
            lblDITVersion.Width = widthB1;
            lblDITDate.Left = leftB2;
            lblDITDate.Width = widthB1;

            lblDITPath.Width = width;

            lblDITRomsGot.Width = widthB1;
            lblDITRomsMissing.Width = widthB1;

            lblDIRomsFixable.Left = leftB2 - backD;
            lblDIRomsUnknown.Left = leftB2 - backD;

            lblDITRomsFixable.Left = leftB2;
            lblDITRomsFixable.Width = widthB1;
            lblDITRomsUnknown.Left = leftB2;
            lblDITRomsUnknown.Width = widthB1;
        }


        #endregion

        #region "Game Grid Code"

        private void UpdateGameGrid(RvDir tDir)
        {


            lblDITName.Text = tDir.Name;
            if (tDir.Dat != null)
            {
                RvDat tDat = tDir.Dat;
                lblDITDescription.Text = tDat.GetData(RvDat.DatData.Description);
                lblDITCategory.Text = tDat.GetData(RvDat.DatData.Category);
                lblDITVersion.Text = tDat.GetData(RvDat.DatData.Version);
                lblDITAuthor.Text = tDat.GetData(RvDat.DatData.Author);
                lblDITDate.Text = tDat.GetData(RvDat.DatData.Date);
            }
            else if (tDir.DirDatCount == 1)
            {
                RvDat tDat = tDir.DirDat(0);
                lblDITDescription.Text = tDat.GetData(RvDat.DatData.Description);
                lblDITCategory.Text = tDat.GetData(RvDat.DatData.Category);
                lblDITVersion.Text = tDat.GetData(RvDat.DatData.Version);
                lblDITAuthor.Text = tDat.GetData(RvDat.DatData.Author);
                lblDITDate.Text = tDat.GetData(RvDat.DatData.Date);
            }
            else
            {
                lblDITDescription.Text = "";
                lblDITCategory.Text = "";
                lblDITVersion.Text = "";
                lblDITAuthor.Text = "";
                lblDITDate.Text = "";
            }



            lblDITPath.Text = tDir.FullName;


            lblDITRomsGot.Text = tDir.DirStatus.CountCorrect().ToString(CultureInfo.InvariantCulture);
            lblDITRomsMissing.Text = tDir.DirStatus.CountMissing().ToString(CultureInfo.InvariantCulture);
            lblDITRomsFixable.Text = tDir.DirStatus.CountFixesNeeded().ToString(CultureInfo.InvariantCulture);
            lblDITRomsUnknown.Text = (tDir.DirStatus.CountUnknown() + tDir.DirStatus.CountInToSort()).ToString(CultureInfo.InvariantCulture);

            _updatingGameGrid = true;

            if (Settings.IsMono)
            {
                if (GameGrid.RowCount > 0)
                    GameGrid.CurrentCell = GameGrid[0,0];
                if (RomGrid.RowCount > 0)
                    RomGrid.CurrentCell = RomGrid[0,0];
            }

            GameGrid.Rows.Clear();
            RomGrid.Rows.Clear();

            // clear sorting
            GameGrid.Columns[GameGridSortColumnIndex].HeaderCell.SortGlyphDirection = SortOrder.None;
            GameGridSortColumnIndex = 0;
            GameGridSortOrder = SortOrder.Descending;


            ReportStatus tDirStat;

            GameGridColumnXPositions = new int[(int)RepStatus.EndValue];

            int rowCount = 0;
            for (int j = 0; j < tDir.ChildCount; j++)
            {

                RvDir tChildDir = tDir.Child(j) as RvDir;
                if (tChildDir == null) continue;

                tDirStat = tChildDir.DirStatus;

                bool gCorrect = tDirStat.HasCorrect();
                bool gMissing = tDirStat.HasMissing();
                bool gUnknown = tDirStat.HasUnknown();
                bool gInToSort = tDirStat.HasInToSort();
                bool gFixes = tDirStat.HasFixesNeeded();

                bool show = (chkBoxShowCorrect.Checked && gCorrect && !gMissing && !gFixes);
                show = show || (chkBoxShowMissing.Checked && gMissing);
                show = show || (chkBoxShowFixed.Checked && gFixes);
                show = show || (gUnknown);
                show = show || (gInToSort);
                show = show || (tChildDir.GotStatus == GotStatus.Corrupt);
                show = show || !(gCorrect || gMissing || gUnknown || gInToSort || gFixes);

                if (!show) continue;

                rowCount++;

                int columnIndex = 0;
                for (int l = 0; l < RepairStatus.DisplayOrder.Length; l++)
                {
                    if (l >= 13) columnIndex = l;

                    if (tDirStat.Get(RepairStatus.DisplayOrder[l]) <= 0) continue;

                    int len = DigitLength(tDirStat.Get(RepairStatus.DisplayOrder[l])) * 7 + 26;
                    if (len > GameGridColumnXPositions[columnIndex])
                        GameGridColumnXPositions[columnIndex] = len;
                    columnIndex++;
                }
            }
            GameGrid.RowCount = rowCount;

            int t = 0;
            for (int l = 0; l < (int)RepStatus.EndValue; l++)
            {
                int colWidth = GameGridColumnXPositions[l];
                GameGridColumnXPositions[l] = t;
                t += colWidth;
            }

            int row = 0;
            for (int j = 0; j < tDir.ChildCount; j++)
            {
                RvDir tChildDir = tDir.Child(j) as RvDir;
                if (tChildDir == null) continue;

                tDirStat = tChildDir.DirStatus;

                bool gCorrect = tDirStat.HasCorrect();
                bool gMissing = tDirStat.HasMissing();
                bool gUnknown = tDirStat.HasUnknown();
                bool gFixes = tDirStat.HasFixesNeeded();
                bool gInToSort = tDirStat.HasInToSort();

                bool show = (chkBoxShowCorrect.Checked && gCorrect && !gMissing && !gFixes);
                show = show || (chkBoxShowMissing.Checked && gMissing);
                show = show || (chkBoxShowFixed.Checked && gFixes);
                show = show || (gUnknown);
                show = show || (gInToSort);
                show = show || (tChildDir.GotStatus == GotStatus.Corrupt);
                show = show || !(gCorrect || gMissing || gUnknown || gInToSort || gFixes);

                if (!show) continue;
               
                GameGrid.Rows[row].Selected = false;
                GameGrid.Rows[row].Tag = tChildDir;
                row++;
            }

            _updatingGameGrid = false;

            UpdateRomGrid(tDir);
        }

        private static int DigitLength(int number)
        {

            int textNumber = number;
            int len = 0;

            while (textNumber > 0)
            {
                textNumber = textNumber / 10;
                len++;
            }

            return len;
        }

        private void GameGridSelectionChanged(object sender, EventArgs e)
        {
            UpdateSelectedGame();
        }

        private void GameGridMouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (_updatingGameGrid)
                return;

            if (GameGrid.SelectedRows.Count != 1)
                return;

            RvDir tGame = (RvDir)GameGrid.SelectedRows[0].Tag;
            if (tGame.Game == null)
            {
                UpdateGameGrid(tGame);
                DirTree.SetSelected(tGame);
            }
        }

        private void GameGrid_CellFormatting(object sender, System.Windows.Forms.DataGridViewCellFormattingEventArgs e)
        {

            if (_updatingGameGrid)
                return;
  
            Rectangle cellBounds = GameGrid.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);
            RvDir tRvDir = (ROMVault2.RvDB.RvDir)GameGrid.Rows[e.RowIndex].Tag;
            ReportStatus tDirStat = tRvDir.DirStatus;
            Color bgCol = Color.FromArgb(255, 255, 255);

            if (cellBounds.Width == 0 || cellBounds.Height == 0)
                return;

            foreach (RepStatus t1 in RepairStatus.DisplayOrder)
            {
                if (tDirStat.Get(t1) <= 0) continue;

                bgCol = _displayColor[(int)t1];
                break;
            }

            if (GameGrid.Columns[e.ColumnIndex].Name == "Type")
            {
                e.CellStyle.BackColor = bgCol;
                e.CellStyle.SelectionBackColor = bgCol;

                Bitmap bmp = new Bitmap(cellBounds.Width, cellBounds.Height);
                Graphics g = Graphics.FromImage(bmp);

                string bitmapName;
                switch (tRvDir.FileType)
                {
                    case FileType.Zip:
                        if (tRvDir.RepStatus == RepStatus.DirCorrect && tRvDir.ZipStatus == ZipStatus.TrrntZip)
                            bitmapName = "ZipTZ";
                        else
                            bitmapName = "Zip" + tRvDir.RepStatus;
                        break;
                    default:
                        // hack because DirDirInToSort image doesnt exist.
                        if (tRvDir.RepStatus ==  RepStatus.DirInToSort)
                            bitmapName = "Dir" + RepStatus.DirUnknown;
                        else
                            bitmapName = "Dir" + tRvDir.RepStatus;

                        break;
                }

                Bitmap bm = rvImages.GetBitmap(bitmapName);
                if (bm != null)
                {
                    g.DrawImage(bm, (cellBounds.Width - cellBounds.Height) / 2, 0, 18, 18);
                    bm.Dispose();
                }
                else
                    Debug.WriteLine("Missing Graphic for " + bitmapName);

                e.Value = bmp;

            } 
            else if (GameGrid.Columns[e.ColumnIndex].Name == "CGame")
            {
                e.CellStyle.BackColor = bgCol;

                if (String.IsNullOrEmpty(tRvDir.FileName))
                    e.Value = tRvDir.Name;
                else
                    e.Value = tRvDir.Name + " (Found: " + tRvDir.FileName + ")";
            }
            else if (GameGrid.Columns[e.ColumnIndex].Name == "CDescription")
            {
                e.CellStyle.BackColor = bgCol;

                if (tRvDir.Game != null)
                    e.Value = tRvDir.Game.GetData(RvGame.GameData.Description);
            }
            else if (GameGrid.Columns[e.ColumnIndex].Name == "CCorrect")
            {
                e.CellStyle.SelectionBackColor = Color.White;

                Bitmap bmp = new Bitmap(cellBounds.Width, cellBounds.Height);
                Graphics g = Graphics.FromImage(bmp);
                g.Clear(Color.White);
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
                Font drawFont = new Font("Arial", 9);
                SolidBrush drawBrushBlack = new SolidBrush(Color.Black);

                int gOff;
                int columnIndex = 0;
                for (int l = 0; l < RepairStatus.DisplayOrder.Length; l++)
                {
                    if (l >= 13) columnIndex = l;

                    if (tRvDir.DirStatus.Get(RepairStatus.DisplayOrder[l]) <= 0) continue;

                    gOff = FrmMain.GameGridColumnXPositions[columnIndex];
                    Bitmap bm = rvImages.GetBitmap(@"G_" + RepairStatus.DisplayOrder[l]);
                    if (bm != null)
                    {
                        g.DrawImage(bm, gOff, 0, 21, 18);
                        bm.Dispose();
                    }
                    else
                        Debug.WriteLine("Missing Graphics for " + "G_" + RepairStatus.DisplayOrder[l]);

                    columnIndex++;
                }

                columnIndex = 0;
                for (int l = 0; l < RepairStatus.DisplayOrder.Length; l++)
                {
                    if (l >= 13)
                        columnIndex = l;

                    if (tRvDir.DirStatus.Get(RepairStatus.DisplayOrder[l]) > 0)
                    {
                        gOff = FrmMain.GameGridColumnXPositions[columnIndex];
                        g.DrawString(tRvDir.DirStatus.Get(RepairStatus.DisplayOrder[l]).ToString(CultureInfo.InvariantCulture), drawFont, drawBrushBlack, new PointF(gOff + 20, 3));
                        columnIndex++;
                    }
                }
                drawBrushBlack.Dispose();
                drawFont.Dispose();
                e.Value = bmp;
            }
            else
                Console.WriteLine("WARN: GameGrid_CellFormatting() unknown column: {0}", GameGrid.Columns[e.ColumnIndex].Name);
        }

        private void GameGridColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {

            // only allow sort on CGame/CDescription
            if (e.ColumnIndex != 1 && e.ColumnIndex != 2)
                return;

            DataGridViewColumn newColumn = GameGrid.Columns[e.ColumnIndex];
            DataGridViewColumn oldColumn = GameGrid.Columns[GameGridSortColumnIndex];

            if (newColumn == oldColumn)
            {
                if (GameGridSortOrder == SortOrder.Ascending)
                    GameGridSortOrder = SortOrder.Descending;
                else
                    GameGridSortOrder = SortOrder.Ascending;
            }
            else
            {
                oldColumn.HeaderCell.SortGlyphDirection = SortOrder.None;
                GameGridSortOrder = SortOrder.Ascending;
            }

            GameGrid.Sort(new GameGridRowComparer(GameGridSortOrder, e.ColumnIndex));
            newColumn.HeaderCell.SortGlyphDirection = GameGridSortOrder;
            GameGridSortColumnIndex = e.ColumnIndex;
        }

        private class GameGridRowComparer : System.Collections.IComparer
        {
            private int sortMod = 1;
            private int columnIndex;

            public GameGridRowComparer(SortOrder sortOrder, int index)
            {
                columnIndex = index;

                if (sortOrder == SortOrder.Descending)
                    sortMod = -1;
            }

            public int Compare(object a, object b)
            {
                DataGridViewRow aRow = (DataGridViewRow)a;
                DataGridViewRow bRow = (DataGridViewRow)b;

                RvDir aRvDir = (ROMVault2.RvDB.RvDir)aRow.Tag;
                RvDir bRvDir = (ROMVault2.RvDB.RvDir)bRow.Tag;

                int result = 0;
                switch (columnIndex)
                {
                    case 1: // CGame
                        result = System.String.Compare(aRvDir.Name, bRvDir.Name);
                        break;
                    case 2: // CDescription
                        String aDes = "";
                        String bDes = "";
                        if (aRvDir.Game != null)
                            aDes = aRvDir.Game.GetData(RvGame.GameData.Description);
                        if (bRvDir.Game != null)
                            bDes = bRvDir.Game.GetData(RvGame.GameData.Description);

                        result = System.String.Compare(aDes, bDes);

                        // if desciptions match, fall through to sorting by name
                        if (result == 0)
                            result = System.String.Compare(aRvDir.Name, bRvDir.Name);

                        break;
                    default:
                        Console.WriteLine("WARN: GameGridRowComparer::Compare() Invalid columnIndex: {0}", columnIndex);
                        break;
                }
                return sortMod * result;
            }
        }
        #endregion

        #region "Rom Grid Code"

        private void splitContainer4_Panel1_Resize(object sender, EventArgs e)
        {
            // fixes a rendering issue in mono
            if (splitContainer4.Panel1.Width == 0) return;

            int chkLeft = splitContainer4.Panel1.Width - 150;
            if (chkLeft < 430) chkLeft = 430;

            chkBoxShowCorrect.Left = chkLeft;
            chkBoxShowMissing.Left = chkLeft;
            chkBoxShowFixed.Left = chkLeft;
            chkBoxShowMerged.Left = chkLeft;
            btnColorKey.Left = chkLeft;

            gbSetInfo.Width = chkLeft - gbSetInfo.Left - 10;
        }


        private void gbSetInfo_Resize(object sender, EventArgs e)
        {
            int leftPos = 84;
            int rightPos = gbSetInfo.Width - 15;
            if (rightPos > 750) rightPos = 750;
            int width = rightPos - leftPos;

            int widthB1 = (int)((double)width * 120 / 340);
            int leftB2 = leftPos + width - widthB1;

            if (lblSITName == null) return;

            lblSITName.Width = width;
            lblSITDescription.Width = width;
            lblSITManufacturer.Width = width;

            lblSITCloneOf.Width = widthB1;

            lblSIYear.Left = leftB2 - 78;
            lblSITYear.Left = leftB2;
            lblSITYear.Width = widthB1;

            lblSITRomOf.Width = widthB1;

            lblSITotalRoms.Left = leftB2 - 78;
            lblSITTotalRoms.Left = leftB2;
            lblSITTotalRoms.Width = widthB1;

            lblSITPublisher.Width = width;
            lblSITDeveloper.Width = width;

            int width3 = (int)((double)width * 0.24);
            int P2 = (int)((double)width * 0.38);

            int width4 = (int) ((double) width*0.24);

            lblSITEdition.Width = width3;

            lblSIVersion.Left = leftPos + P2 - 78;
            lblSITVersion.Left = leftPos + P2;
            lblSITVersion.Width = width3;

            lblSIType.Left = leftPos + width - width3 - 78;
            lblSITType.Left = leftPos + width - width3;
            lblSITType.Width = width3;


            lblSITMedia.Width = width3;

            lblSILanguage.Left = leftPos + P2 - 78;
            lblSITLanguage.Left = leftPos + P2;
            lblSITLanguage.Width = width3;

            lblSIPlayers.Left = leftPos + width - width3 - 78;
            lblSITPlayers.Left = leftPos + width - width3;
            lblSITPlayers.Width = width3;
            
            lblSITRatings.Width = width3;

            lblSIGenre.Left = leftPos + P2 - 78;
            lblSITGenre.Left = leftPos + P2;
            lblSITGenre.Width = width3;

            lblSIPeripheral.Left = leftPos + width - width3 - 78;
            lblSITPeripheral.Left = leftPos + width - width3;
            lblSITPeripheral.Width = width3;

            
            lblSITBarCode.Width = width4;

            lblSIMediaCatalogNumber.Left = leftPos + width - width4 - 78;
            lblSITMediaCatalogNumber.Left = leftPos + width - width4;
            lblSITMediaCatalogNumber.Width = width4;


        }


        private void UpdateSelectedGame()
        {
            if (_updatingGameGrid)
                return;

            if (GameGrid.SelectedRows.Count != 1)
                return;

            RvDir tGame = (RvDir)GameGrid.SelectedRows[0].Tag;
            UpdateRomGrid(tGame);
        }

        private void UpdateRomGrid(RvDir tGame)
        {
            lblSITName.Text = tGame.Name;
            if (tGame.Game == null)
            {
                lblSIDescription.Visible = false;
                lblSITDescription.Visible = false;

                lblSIManufacturer.Visible = false;
                lblSITManufacturer.Visible = false;

                lblSICloneOf.Visible = false;
                lblSITCloneOf.Visible = false;

                lblSIRomOf.Visible = false;
                lblSITRomOf.Visible = false;

                lblSIYear.Visible = false;
                lblSITYear.Visible = false;

                lblSITotalRoms.Visible = false;
                lblSITTotalRoms.Visible = false;

                // Trurip

                lblSIPublisher.Visible = false;
                lblSITPublisher.Visible = false;

                lblSIDeveloper.Visible = false;
                lblSITDeveloper.Visible = false;

                lblSIEdition.Visible = false;
                lblSITEdition.Visible = false;

                lblSIVersion.Visible = false;
                lblSITVersion.Visible = false;

                lblSIType.Visible = false;
                lblSITType.Visible = false;

                lblSIMedia.Visible = false;
                lblSITMedia.Visible = false;

                lblSILanguage.Visible = false;
                lblSITLanguage.Visible = false;

                lblSIPlayers.Visible = false;
                lblSITPlayers.Visible = false;

                lblSIRatings.Visible = false;
                lblSITRatings.Visible = false;

                lblSIGenre.Visible = false;
                lblSITGenre.Visible = false;

                lblSIPeripheral.Visible = false;
                lblSITPeripheral.Visible = false;

                lblSIBarCode.Visible = false;
                lblSITBarCode.Visible = false;

                lblSIMediaCatalogNumber.Visible = false;
                lblSITMediaCatalogNumber.Visible = false;
            }
            else
            {

                if (tGame.Game.GetData(RvGame.GameData.Trurip) == "yes")
                {
                    lblSIDescription.Visible = true;
                    lblSITDescription.Visible = true;
                    lblSITDescription.Text = tGame.Game.GetData(RvGame.GameData.Description);

                    lblSIManufacturer.Visible = false;
                    lblSITManufacturer.Visible = false;

                    lblSICloneOf.Visible = false;
                    lblSITCloneOf.Visible = false;

                    lblSIRomOf.Visible = false;
                    lblSITRomOf.Visible = false;

                    lblSIYear.Visible = false;
                    lblSITYear.Visible = false;

                    lblSITotalRoms.Visible = false;
                    lblSITTotalRoms.Visible = false;


                    lblSIPublisher.Visible = true;
                    lblSITPublisher.Visible = true;
                    lblSITPublisher.Text = tGame.Game.GetData(RvGame.GameData.Publisher);

                    lblSIDeveloper.Visible = true;
                    lblSITDeveloper.Visible = true;
                    lblSITDeveloper.Text = tGame.Game.GetData(RvGame.GameData.Developer);

                    lblSIEdition.Visible = true;
                    lblSITEdition.Visible = true;
                    lblSITEdition.Text = tGame.Game.GetData(RvGame.GameData.Edition);

                    lblSIVersion.Visible = true;
                    lblSITVersion.Visible = true;
                    lblSITVersion.Text = tGame.Game.GetData(RvGame.GameData.Version);

                    lblSIType.Visible = true;
                    lblSITType.Visible = true;
                    lblSITType.Text = tGame.Game.GetData(RvGame.GameData.Type);

                    lblSIMedia.Visible = true;
                    lblSITMedia.Visible = true;
                    lblSITMedia.Text = tGame.Game.GetData(RvGame.GameData.Media);

                    lblSILanguage.Visible = true;
                    lblSITLanguage.Visible = true;
                    lblSITLanguage.Text = tGame.Game.GetData(RvGame.GameData.Language);

                    lblSIPlayers.Visible = true;
                    lblSITPlayers.Visible = true;
                    lblSITPlayers.Text = tGame.Game.GetData(RvGame.GameData.Players);

                    lblSIRatings.Visible = true;
                    lblSITRatings.Visible = true;
                    lblSITRatings.Text = tGame.Game.GetData(RvGame.GameData.Ratings);

                    lblSIGenre.Visible = true;
                    lblSITGenre.Visible = true;
                    lblSITGenre.Text = tGame.Game.GetData(RvGame.GameData.Genre);

                    lblSIPeripheral.Visible = true;
                    lblSITPeripheral.Visible = true;
                    lblSITPeripheral.Text = tGame.Game.GetData(RvGame.GameData.Peripheral);

                    lblSIBarCode.Visible = true;
                    lblSITBarCode.Visible = true;
                    lblSITBarCode.Text = tGame.Game.GetData(RvGame.GameData.BarCode);

                    lblSIMediaCatalogNumber.Visible = true;
                    lblSITMediaCatalogNumber.Visible = true;
                    lblSITMediaCatalogNumber.Text = tGame.Game.GetData(RvGame.GameData.MediaCatalogNumber);

                }
                else
                {
                    lblSIDescription.Visible = true;
                    lblSITDescription.Visible = true;
                    lblSITDescription.Text = tGame.Game.GetData(RvGame.GameData.Description);

                    lblSIManufacturer.Visible = true;
                    lblSITManufacturer.Visible = true;
                    lblSITManufacturer.Text = tGame.Game.GetData(RvGame.GameData.Manufacturer);

                    lblSICloneOf.Visible = true;
                    lblSITCloneOf.Visible = true;
                    lblSITCloneOf.Text = tGame.Game.GetData(RvGame.GameData.CloneOf);

                    lblSIRomOf.Visible = true;
                    lblSITRomOf.Visible = true;
                    lblSITRomOf.Text = tGame.Game.GetData(RvGame.GameData.RomOf);

                    lblSIYear.Visible = true;
                    lblSITYear.Visible = true;
                    lblSITYear.Text = tGame.Game.GetData(RvGame.GameData.Year);

                    lblSITotalRoms.Visible = true;
                    lblSITTotalRoms.Visible = true;




                    lblSIPublisher.Visible = false;
                    lblSITPublisher.Visible = false;

                    lblSIDeveloper.Visible = false;
                    lblSITDeveloper.Visible = false;

                    lblSIEdition.Visible = false;
                    lblSITEdition.Visible = false;

                    lblSIVersion.Visible = false;
                    lblSITVersion.Visible = false;

                    lblSIType.Visible = false;
                    lblSITType.Visible = false;

                    lblSIMedia.Visible = false;
                    lblSITMedia.Visible = false;

                    lblSILanguage.Visible = false;
                    lblSITLanguage.Visible = false;

                    lblSIPlayers.Visible = false;
                    lblSITPlayers.Visible = false;

                    lblSIRatings.Visible = false;
                    lblSITRatings.Visible = false;

                    lblSIGenre.Visible = false;
                    lblSITGenre.Visible = false;

                    lblSIPeripheral.Visible = false;
                    lblSITPeripheral.Visible = false;

                    lblSIBarCode.Visible = false;
                    lblSITBarCode.Visible = false;

                    lblSIMediaCatalogNumber.Visible = false;
                    lblSITMediaCatalogNumber.Visible = false;

                }
            }

            if (Settings.IsMono && RomGrid.RowCount > 0)
                RomGrid.CurrentCell = RomGrid[0,0];

            RomGrid.Rows.Clear();
            AddDir(tGame, "");
            GC.Collect();
        }


        private void AddDir(RvDir tGame, string pathAdd)
        {
            for (int l = 0; l < tGame.ChildCount; l++)
            {
                RvBase tBase = tGame.Child(l);

                RvFile tFile = tBase as RvFile;
                if (tFile != null)
                    AddRom((RvFile)tBase, pathAdd);

                if (tGame.Dat == null)
                    continue;

                RvDir tDir = tBase as RvDir;
                if (tDir == null) continue;
                if (tDir.Game == null)
                    AddDir(tDir, pathAdd + tGame.Name + "/");
            }
        }

        private void AddRom(RvFile tRomTable, string pathAdd)
        {

            if (tRomTable.DatStatus != DatStatus.InDatMerged || tRomTable.RepStatus != RepStatus.NotCollected || chkBoxShowMerged.Checked)
            {
                RomGrid.Rows.Add();
                int row = RomGrid.Rows.Count - 1;
                RomGrid.Rows[row].Tag = tRomTable;

                for (int i = 0; i < RomGrid.Rows[row].Cells.Count; i++)
                    RomGrid.Rows[row].Cells[i].Style.BackColor = _displayColor[(int)tRomTable.RepStatus];

                string fname = pathAdd + tRomTable.Name;
                if (!string.IsNullOrEmpty(tRomTable.FileName))
                    fname += " (Found: " + tRomTable.FileName + ")";
                if (tRomTable.CHDVersion != null)
                    fname += " (V" + tRomTable.CHDVersion + ")";

                RomGrid.Rows[row].Cells["CRom"].Value = fname;

                RomGrid.Rows[row].Cells["CMerge"].Value = tRomTable.Merge;
                RomGrid.Rows[row].Cells["CStatus"].Value = tRomTable.Status;

                string strSize = tRomTable.Size.ToString();
                string flags = "";
                if (tRomTable.FileStatusIs(FileStatus.SizeFromDAT)) flags += "D";
                if (tRomTable.FileStatusIs(FileStatus.SizeFromHeader)) flags += "H";
                if (tRomTable.FileStatusIs(FileStatus.SizeVerified)) flags += "V";
                if (!String.IsNullOrEmpty(flags)) strSize += " (" + flags + ")";
                RomGrid.Rows[row].Cells["CSize"].Value = strSize;

                string strCRC = ArrByte.ToString(tRomTable.CRC);
                flags = "";
                if (tRomTable.FileStatusIs(FileStatus.CRCFromDAT)) flags += "D";
                if (tRomTable.FileStatusIs(FileStatus.CRCFromHeader)) flags += "H";
                if (tRomTable.FileStatusIs(FileStatus.CRCVerified)) flags += "V";
                if (!String.IsNullOrEmpty(flags)) strCRC += " (" + flags + ")";
                RomGrid.Rows[row].Cells["CCRC32"].Value = strCRC;

                if (tRomTable.SHA1CHD == null)
                {
                    string strSHA1 = ArrByte.ToString(tRomTable.SHA1);
                    flags = "";
                    if (tRomTable.FileStatusIs(FileStatus.SHA1FromDAT)) flags += "D";
                    if (tRomTable.FileStatusIs(FileStatus.SHA1FromHeader)) flags += "H";
                    if (tRomTable.FileStatusIs(FileStatus.SHA1Verified)) flags += "V";
                    if (!String.IsNullOrEmpty(flags)) strSHA1 += " (" + flags + ")";
                    RomGrid.Rows[row].Cells["CSHA1"].Value = strSHA1;
                }
                else
                {
                    string strSHA1CHD = "CHD: " + ArrByte.ToString(tRomTable.SHA1CHD);
                    flags = "";
                    if (tRomTable.FileStatusIs(FileStatus.SHA1CHDFromDAT)) flags += "D";
                    if (tRomTable.FileStatusIs(FileStatus.SHA1CHDFromHeader)) flags += "H";
                    if (tRomTable.FileStatusIs(FileStatus.SHA1CHDVerified)) flags += "V";
                    if (!String.IsNullOrEmpty(flags)) strSHA1CHD += " (" + flags + ")";
                    RomGrid.Rows[row].Cells["CSHA1"].Value = strSHA1CHD;
                }

                if (tRomTable.MD5CHD == null)
                {
                    string strMD5 = ArrByte.ToString(tRomTable.MD5);
                    flags = "";
                    if (tRomTable.FileStatusIs(FileStatus.MD5FromDAT)) flags += "D";
                    if (tRomTable.FileStatusIs(FileStatus.MD5FromHeader)) flags += "H";
                    if (tRomTable.FileStatusIs(FileStatus.MD5Verified)) flags += "V";
                    if (!String.IsNullOrEmpty(flags)) strMD5 += " (" + flags + ")";
                    RomGrid.Rows[row].Cells["CMD5"].Value = strMD5;
                }
                else
                {
                    string strMD5CHD = "CHD: " + ArrByte.ToString(tRomTable.MD5CHD);
                    flags = "";
                    if (tRomTable.FileStatusIs(FileStatus.MD5CHDFromDAT)) flags += "D";
                    if (tRomTable.FileStatusIs(FileStatus.MD5CHDFromHeader)) flags += "H";
                    if (tRomTable.FileStatusIs(FileStatus.MD5CHDVerified)) flags += "V";
                    if (!String.IsNullOrEmpty(flags)) strMD5CHD += " (" + flags + ")";
                    RomGrid.Rows[row].Cells["CMD5"].Value = strMD5CHD;
                }

                if (tRomTable.FileType == FileType.ZipFile)
                {
                    RomGrid.Rows[row].Cells["ZipIndex"].Value = tRomTable.ZipFileIndex == -1 ? "" : tRomTable.ZipFileIndex.ToString(CultureInfo.InvariantCulture);
                    RomGrid.Rows[row].Cells["ZipHeader"].Value = tRomTable.ZipFileHeaderPosition == null ? "" : tRomTable.ZipFileHeaderPosition.ToString();
                }
            }
        }

        private void RomGrid_CellFormatting(object sender, System.Windows.Forms.DataGridViewCellFormattingEventArgs e)
        {

            if (_updatingGameGrid)
                return;

            Rectangle cellBounds = RomGrid.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);
            RvFile tRvFile = (ROMVault2.RvDB.RvFile)RomGrid.Rows[e.RowIndex].Tag;

            if (cellBounds.Width == 0 || cellBounds.Height == 0)
                return;

            if (RomGrid.Columns[e.ColumnIndex].Name == "CGot")
            {
                Bitmap bmp = new Bitmap(cellBounds.Width, cellBounds.Height);
                Graphics g = Graphics.FromImage(bmp);
                string bitmapName = "R_" + tRvFile.DatStatus + "_" + tRvFile.RepStatus;
                g.DrawImage(rvImages.GetBitmap(bitmapName), 0, 0, 54, 18);
                e.Value = bmp;
            }
        }

        #endregion

        private void RomGridSelectionChanged(object sender, EventArgs e)
        {
            RomGrid.ClearSelection();
        }

        private void ChkBoxShowCorrectCheckedChanged(object sender, EventArgs e)
        {
            DatSetSelected(DirTree.Selected);
        }

        private void ChkBoxShowMissingCheckedChanged(object sender, EventArgs e)
        {
            DatSetSelected(DirTree.Selected);
        }

        private void ChkBoxShowFixedCheckedChanged(object sender, EventArgs e)
        {
            DatSetSelected(DirTree.Selected);
        }

        private void ChkBoxShowMergedCheckedChanged(object sender, EventArgs e)
        {
            UpdateSelectedGame();
        }

        private void BtnColorKeyClick(object sender, EventArgs e)
        {

            if (_fk == null || _fk.IsDisposed)
                _fk = new FrmKey();
            _fk.Show();
        }

        private void SettingsToolStripMenuItemClick(object sender, EventArgs e)
        {
            FrmSettings fcfg = new FrmSettings();
            fcfg.ShowDialog(this);
            fcfg.Dispose();
        }

        private void BtnReportClick(object sender, EventArgs e)
        {
            Report.MakeFixFiles();
            //FrmReport newreporter = new FrmReport();
            //newreporter.ShowDialog();
            //newreporter.Dispose();
        }
        private void fixDatReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Report.MakeFixFiles();
        }

        private void fullReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Report.GenerateReport();
        }
        private void fixReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Report.GenerateFixReport();
        }


        private void AboutRomVaultToolStripMenuItemClick(object sender, EventArgs e)
        {
            FrmHelpAbout fha = new FrmHelpAbout();
            fha.ShowDialog(this);
            fha.Dispose();
        }

        private void RomGridMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int currentMouseOverRow = RomGrid.HitTest(e.X, e.Y).RowIndex;
                if (currentMouseOverRow >= 0)
                {
                    string name = RomGrid.Rows[currentMouseOverRow].Cells[1].Value.ToString();
                    string size = RomGrid.Rows[currentMouseOverRow].Cells[2].Value.ToString();
                    string crc = RomGrid.Rows[currentMouseOverRow].Cells[4].Value.ToString();
                    if (crc.Length > 8) crc = crc.Substring(0, 8);
                    string sha1 = RomGrid.Rows[currentMouseOverRow].Cells[5].Value.ToString();
                    if (sha1.Length > 40) sha1 = sha1.Substring(0, 40);
                    string md5 = RomGrid.Rows[currentMouseOverRow].Cells[6].Value.ToString();
                    if (md5.Length > 32) md5 = md5.Substring(0, 32);

                    string clipText = "Name : " + name + Environment.NewLine;
                    clipText += "Size : " + size + Environment.NewLine;
                    clipText += "CRC32: " + crc + Environment.NewLine;
                    if (sha1.Length > 0) clipText += "SHA1 : " + sha1 + Environment.NewLine;
                    if (md5.Length > 0) clipText += "MD5  : " + md5 + Environment.NewLine;

                    try
                    {
                        Clipboard.Clear();
                        Clipboard.SetText(clipText);
                    }
                    catch
                    {
                    }
                }
            }
        }

        private void GameGrid_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int currentMouseOverRow = GameGrid.HitTest(e.X, e.Y).RowIndex;
                if (currentMouseOverRow >= 0)
                {
                    object r1 = GameGrid.Rows[currentMouseOverRow].Cells[1].Value;
                    string filename = r1 != null ? r1.ToString() : "";
                    object r2 = GameGrid.Rows[currentMouseOverRow].Cells[2].Value;
                    string description = r2 != null ? r2.ToString() : "";

                    try
                    {
                        Clipboard.Clear();
                        Clipboard.SetText("Name : " + filename + Environment.NewLine + "Desc : " + description + Environment.NewLine);
                    }
                    catch
                    {
                    }
                }
            }

        }
        
    }
}
