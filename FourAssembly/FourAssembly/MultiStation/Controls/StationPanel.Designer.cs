namespace FourAssembly.MultiStation.Controls
{
    partial class StationPanel
    {
        private System.ComponentModel.IContainer components = null;
        private Label _lblStation;
        private TextBox _txtScanInput;
        private ListView _lstCables;
        private Label _lblScrapCount;
        private Label _lblInsertedCount;
        private Label _lblGoodPieceCount;
        private Button _btnResetGoodCount;
        private Button _btnFinalize;
        private Button _btnReset;
        private Button _btnAdminRecoveryReset;
        private Label _lblStatus;
        private Label _lblApiInfo;
        private TextBox _txtApiInfo;
        private Panel pnlMasterStatus;
        private Label lblMasterPartNumber;
        private Label lblMasterStatus;
        private Label lblMasterProcessState;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            _lblStation = new Label();
            _txtScanInput = new TextBox();
            _lstCables = new ListView();
            _lblScrapCount = new Label();
            _lblInsertedCount = new Label();
            _lblGoodPieceCount = new Label();
            _btnResetGoodCount = new Button();
            _btnFinalize = new Button();
            _btnReset = new Button();
            _btnAdminRecoveryReset = new Button();
            _lblStatus = new Label();
            _lblApiInfo = new Label();
            _txtApiInfo = new TextBox();
            lblInfo = new Label();
            lblBG = new Label();
            panel3 = new Panel();
            panel4 = new Panel();
            pnlMasterStatus = new Panel();
            lblMasterProcessState = new Label();
            lblMasterStatus = new Label();
            lblMasterPartNumber = new Label();
            panel3.SuspendLayout();
            pnlMasterStatus.SuspendLayout();
            SuspendLayout();
            // 
            // _lblStation
            // 
            _lblStation.Dock = DockStyle.Fill;
            _lblStation.Font = new Font("Malgun Gothic", 27.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            _lblStation.ForeColor = Color.FromArgb(20, 55, 100);
            _lblStation.Location = new Point(0, 0);
            _lblStation.Name = "_lblStation";
            _lblStation.Size = new Size(1079, 98);
            _lblStation.TabIndex = 0;
            _lblStation.Text = "Station";
            _lblStation.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // _txtScanInput
            // 
            _txtScanInput.Dock = DockStyle.Bottom;
            _txtScanInput.Font = new Font("Consolas", 11F);
            _txtScanInput.Location = new Point(0, 827);
            _txtScanInput.Name = "_txtScanInput";
            _txtScanInput.Size = new Size(1343, 25);
            _txtScanInput.TabIndex = 3;
            _txtScanInput.KeyDown += _txtScanInput_KeyDown;
            // 
            // _lstCables
            // 
            _lstCables.Dock = DockStyle.Top;
            _lstCables.Font = new Font("Segoe UI", 24F, FontStyle.Regular, GraphicsUnit.Point, 0);
            _lstCables.Location = new Point(0, 346);
            _lstCables.Name = "_lstCables";
            _lstCables.Size = new Size(1343, 91);
            _lstCables.TabIndex = 5;
            _lstCables.UseCompatibleStateImageBehavior = false;
            _lstCables.View = View.List;
            // 
            // _lblScrapCount
            // 
            _lblScrapCount.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            _lblScrapCount.BackColor = Color.MistyRose;
            _lblScrapCount.BorderStyle = BorderStyle.FixedSingle;
            _lblScrapCount.Font = new Font("Segoe UI", 15F, FontStyle.Bold, GraphicsUnit.Point, 0);
            _lblScrapCount.ForeColor = Color.Red;
            _lblScrapCount.Location = new Point(1043, 569);
            _lblScrapCount.Name = "_lblScrapCount";
            _lblScrapCount.Size = new Size(285, 52);
            _lblScrapCount.TabIndex = 19;
            _lblScrapCount.Text = "Scrap: 0";
            _lblScrapCount.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // _lblInsertedCount
            // 
            _lblInsertedCount.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            _lblInsertedCount.BackColor = Color.Honeydew;
            _lblInsertedCount.BorderStyle = BorderStyle.FixedSingle;
            _lblInsertedCount.Font = new Font("Segoe UI", 15F, FontStyle.Bold, GraphicsUnit.Point, 0);
            _lblInsertedCount.ForeColor = Color.DarkGreen;
            _lblInsertedCount.Location = new Point(1043, 627);
            _lblInsertedCount.Name = "_lblInsertedCount";
            _lblInsertedCount.Size = new Size(285, 52);
            _lblInsertedCount.TabIndex = 20;
            _lblInsertedCount.Text = "Total de piezas 0/0";
            _lblInsertedCount.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // _lblGoodPieceCount
            // 
            _lblGoodPieceCount.BackColor = Color.Honeydew;
            _lblGoodPieceCount.BorderStyle = BorderStyle.FixedSingle;
            _lblGoodPieceCount.Dock = DockStyle.Right;
            _lblGoodPieceCount.Font = new Font("Segoe UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            _lblGoodPieceCount.ForeColor = Color.DarkGreen;
            _lblGoodPieceCount.Location = new Point(1079, 0);
            _lblGoodPieceCount.Name = "_lblGoodPieceCount";
            _lblGoodPieceCount.Size = new Size(264, 98);
            _lblGoodPieceCount.TabIndex = 21;
            _lblGoodPieceCount.Text = "Total estación: 0\r\nBG --: 0\r\nHora 00:00-01:00: 0";
            _lblGoodPieceCount.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // _btnResetGoodCount
            // 
            _btnResetGoodCount.BackColor = Color.WhiteSmoke;
            _btnResetGoodCount.Dock = DockStyle.Bottom;
            _btnResetGoodCount.FlatStyle = FlatStyle.Flat;
            _btnResetGoodCount.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            _btnResetGoodCount.Location = new Point(0, 41);
            _btnResetGoodCount.Name = "_btnResetGoodCount";
            _btnResetGoodCount.Size = new Size(216, 39);
            _btnResetGoodCount.TabIndex = 22;
            _btnResetGoodCount.Text = "Reset piezas";
            _btnResetGoodCount.UseVisualStyleBackColor = false;
            _btnResetGoodCount.Visible = false;
            _btnResetGoodCount.Click += _btnResetGoodCount_Click;
            // 
            // _btnFinalize
            // 
            _btnFinalize.BackColor = Color.LimeGreen;
            _btnFinalize.Location = new Point(1176, 639);
            _btnFinalize.Name = "_btnFinalize";
            _btnFinalize.Size = new Size(100, 40);
            _btnFinalize.TabIndex = 7;
            _btnFinalize.Text = "Finalize";
            _btnFinalize.UseVisualStyleBackColor = false;
            _btnFinalize.Visible = false;
            _btnFinalize.Click += _btnFinalize_Click;
            // 
            // _btnReset
            // 
            _btnReset.Dock = DockStyle.Top;
            _btnReset.FlatAppearance.BorderColor = Color.Maroon;
            _btnReset.FlatStyle = FlatStyle.Flat;
            _btnReset.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            _btnReset.ForeColor = Color.Maroon;
            _btnReset.Location = new Point(0, 0);
            _btnReset.Name = "_btnReset";
            _btnReset.Size = new Size(216, 39);
            _btnReset.TabIndex = 8;
            _btnReset.Text = "Reset general";
            _btnReset.UseVisualStyleBackColor = true;
            _btnReset.Visible = false;
            _btnReset.Click += _btnReset_Click;
            // 
            // _btnAdminRecoveryReset
            // 
            _btnAdminRecoveryReset.BackColor = Color.White;
            _btnAdminRecoveryReset.Dock = DockStyle.Bottom;
            _btnAdminRecoveryReset.FlatAppearance.BorderSize = 0;
            _btnAdminRecoveryReset.FlatStyle = FlatStyle.Flat;
            _btnAdminRecoveryReset.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            _btnAdminRecoveryReset.ForeColor = Color.Gray;
            _btnAdminRecoveryReset.Location = new Point(0, 797);
            _btnAdminRecoveryReset.Name = "_btnAdminRecoveryReset";
            _btnAdminRecoveryReset.Size = new Size(1343, 30);
            _btnAdminRecoveryReset.TabIndex = 24;
            _btnAdminRecoveryReset.Text = "Four Assembly";
            _btnAdminRecoveryReset.TextImageRelation = TextImageRelation.TextAboveImage;
            _btnAdminRecoveryReset.UseVisualStyleBackColor = false;
            _btnAdminRecoveryReset.Click += _btnAdminRecoveryReset_Click;
            // 
            // _lblStatus
            // 
            _lblStatus.AutoSize = true;
            _lblStatus.ForeColor = Color.Blue;
            _lblStatus.Location = new Point(20, 540);
            _lblStatus.Name = "_lblStatus";
            _lblStatus.Size = new Size(0, 15);
            _lblStatus.TabIndex = 9;
            // 
            // _lblApiInfo
            // 
            _lblApiInfo.AutoSize = true;
            _lblApiInfo.Dock = DockStyle.Top;
            _lblApiInfo.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            _lblApiInfo.ForeColor = Color.FromArgb(20, 55, 100);
            _lblApiInfo.Location = new Point(0, 325);
            _lblApiInfo.Name = "_lblApiInfo";
            _lblApiInfo.Size = new Size(71, 21);
            _lblApiInfo.TabIndex = 17;
            _lblApiInfo.Text = "API Info";
            // 
            // _txtApiInfo
            // 
            _txtApiInfo.Font = new Font("Consolas", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            _txtApiInfo.Location = new Point(3, 515);
            _txtApiInfo.Multiline = true;
            _txtApiInfo.Name = "_txtApiInfo";
            _txtApiInfo.ReadOnly = true;
            _txtApiInfo.ScrollBars = ScrollBars.Vertical;
            _txtApiInfo.Size = new Size(1034, 270);
            _txtApiInfo.TabIndex = 18;
            // 
            // lblInfo
            // 
            lblInfo.Dock = DockStyle.Top;
            lblInfo.Font = new Font("Malgun Gothic", 30F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblInfo.ForeColor = Color.DarkOrange;
            lblInfo.Location = new Point(0, 254);
            lblInfo.Name = "lblInfo";
            lblInfo.Size = new Size(1343, 71);
            lblInfo.TabIndex = 14;
            lblInfo.Text = "--";
            lblInfo.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblBG
            // 
            lblBG.Dock = DockStyle.Top;
            lblBG.Font = new Font("Malgun Gothic", 30F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblBG.ForeColor = Color.DarkOrange;
            lblBG.Location = new Point(0, 178);
            lblBG.Name = "lblBG";
            lblBG.Size = new Size(1343, 76);
            lblBG.TabIndex = 15;
            lblBG.Text = "--";
            lblBG.TextAlign = ContentAlignment.MiddleCenter;
            lblBG.Click += lblBG_Click;
            // 
            // panel3
            // 
            panel3.BackColor = Color.White;
            panel3.Controls.Add(_lblStation);
            panel3.Controls.Add(_lblGoodPieceCount);
            panel3.Dock = DockStyle.Top;
            panel3.Location = new Point(0, 80);
            panel3.Name = "panel3";
            panel3.Size = new Size(1343, 98);
            panel3.TabIndex = 14;
            panel3.Paint += panel3_Paint;
            // 
            // panel4
            // 
            panel4.BackColor = Color.White;
            panel4.Dock = DockStyle.Right;
            panel4.Location = new Point(1127, 0);
            panel4.Name = "panel4";
            panel4.Size = new Size(216, 80);
            panel4.TabIndex = 15;
            // 
            // pnlMasterStatus
            // 
            pnlMasterStatus.BackColor = Color.DarkGray;
            pnlMasterStatus.Controls.Add(lblMasterProcessState);
            pnlMasterStatus.Controls.Add(lblMasterStatus);
            pnlMasterStatus.Controls.Add(lblMasterPartNumber);
            pnlMasterStatus.Dock = DockStyle.Top;
            pnlMasterStatus.Location = new Point(0, 0);
            pnlMasterStatus.Name = "pnlMasterStatus";
            pnlMasterStatus.Size = new Size(1343, 80);
            pnlMasterStatus.TabIndex = 23;
            // 
            // lblMasterProcessState
            // 
            lblMasterProcessState.AutoSize = true;
            lblMasterProcessState.Font = new Font("Segoe UI", 10F);
            lblMasterProcessState.ForeColor = Color.Orange;
            lblMasterProcessState.Location = new Point(350, 15);
            lblMasterProcessState.Name = "lblMasterProcessState";
            lblMasterProcessState.Size = new Size(134, 19);
            lblMasterProcessState.TabIndex = 2;
            lblMasterProcessState.Text = "Esperando proceso...";
            // 
            // lblMasterStatus
            // 
            lblMasterStatus.AutoSize = true;
            lblMasterStatus.Font = new Font("Segoe UI", 11F);
            lblMasterStatus.ForeColor = Color.White;
            lblMasterStatus.Location = new Point(10, 50);
            lblMasterStatus.Name = "lblMasterStatus";
            lblMasterStatus.Size = new Size(124, 20);
            lblMasterStatus.TabIndex = 1;
            lblMasterStatus.Text = "Status: DISABLED";
            // 
            // lblMasterPartNumber
            // 
            lblMasterPartNumber.AutoSize = true;
            lblMasterPartNumber.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            lblMasterPartNumber.ForeColor = Color.White;
            lblMasterPartNumber.Location = new Point(10, 7);
            lblMasterPartNumber.Name = "lblMasterPartNumber";
            lblMasterPartNumber.Size = new Size(213, 37);
            lblMasterPartNumber.TabIndex = 0;
            lblMasterPartNumber.Text = "PartNumber: --";
            // 
            // StationPanel
            // 
            BackColor = Color.White;
            Controls.Add(_lstCables);
            Controls.Add(_btnAdminRecoveryReset);
            Controls.Add(_lblApiInfo);
            Controls.Add(lblInfo);
            Controls.Add(_txtScanInput);
            Controls.Add(_lblScrapCount);
            Controls.Add(_lblInsertedCount);
            Controls.Add(_txtApiInfo);
            Controls.Add(_btnFinalize);
            Controls.Add(_lblStatus);
            Controls.Add(lblBG);
            Controls.Add(panel3);
            Controls.Add(pnlMasterStatus);
            Name = "StationPanel";
            Size = new Size(1343, 852);
            Load += StationPanel_Load;
            panel3.ResumeLayout(false);
            pnlMasterStatus.ResumeLayout(false);
            pnlMasterStatus.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }
        private Label lblInfo;
        private Label lblBG;
        private Panel panel3;
        private Panel panel4;
    }
}
