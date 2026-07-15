namespace FourAssembly.MultiStation.Controls
{
    partial class StationPanel
    {
        private System.ComponentModel.IContainer components = null;
        private Label _lblStation;
        private TextBox _txtScanInput;
        private ListView _lstCables;
        private Label _lblCableCount;
        private Label _lblScrapCount;
        private Label _lblInsertedCount;
        private Button _btnReviewApi;
        private Button _btnFinalize;
        private Button _btnReset;
        private Label _lblStatus;
        private Label _lblApiInfo;
        private TextBox _txtApiInfo;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StationPanel));
            _lblStation = new Label();
            _txtScanInput = new TextBox();
            _lstCables = new ListView();
            _lblCableCount = new Label();
            _lblScrapCount = new Label();
            _lblInsertedCount = new Label();
            _btnReviewApi = new Button();
            _btnFinalize = new Button();
            _btnReset = new Button();
            _lblStatus = new Label();
            _lblApiInfo = new Label();
            _txtApiInfo = new TextBox();
            panel1 = new Panel();
            button1 = new Button();
            panel2 = new Panel();
            lblInfo = new Label();
            lblBG = new Label();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // _lblStation
            // 
            _lblStation.Dock = DockStyle.Top;
            _lblStation.Font = new Font("Malgun Gothic", 27.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            _lblStation.ForeColor = Color.FromArgb(20, 55, 100);
            _lblStation.Location = new Point(0, 75);
            _lblStation.Name = "_lblStation";
            _lblStation.Size = new Size(1343, 91);
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
            _lstCables.Location = new Point(20, 394);
            _lstCables.Name = "_lstCables";
            _lstCables.Size = new Size(672, 224);
            _lstCables.TabIndex = 5;
            _lstCables.UseCompatibleStateImageBehavior = false;
            _lstCables.View = View.List;
            // 
            // _lblCableCount
            // 
            _lblCableCount.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            _lblCableCount.BackColor = Color.FromArgb(245, 248, 252);
            _lblCableCount.BorderStyle = BorderStyle.FixedSingle;
            _lblCableCount.Font = new Font("Segoe UI", 15F, FontStyle.Bold, GraphicsUnit.Point, 0);
            _lblCableCount.ForeColor = Color.FromArgb(20, 55, 100);
            _lblCableCount.Location = new Point(1030, 562);
            _lblCableCount.Name = "_lblCableCount";
            _lblCableCount.Size = new Size(285, 52);
            _lblCableCount.TabIndex = 6;
            _lblCableCount.Text = "Escaneo total: 0";
            _lblCableCount.TextAlign = ContentAlignment.MiddleCenter;
            _lblCableCount.Visible = false;
            // 
            // _lblScrapCount
            // 
            _lblScrapCount.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            _lblScrapCount.BackColor = Color.MistyRose;
            _lblScrapCount.BorderStyle = BorderStyle.FixedSingle;
            _lblScrapCount.Font = new Font("Segoe UI", 15F, FontStyle.Bold, GraphicsUnit.Point, 0);
            _lblScrapCount.ForeColor = Color.Red;
            _lblScrapCount.Location = new Point(1030, 620);
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
            _lblInsertedCount.Location = new Point(1030, 678);
            _lblInsertedCount.Name = "_lblInsertedCount";
            _lblInsertedCount.Size = new Size(285, 52);
            _lblInsertedCount.TabIndex = 20;
            _lblInsertedCount.Text = "Total de piezas 0/0";
            _lblInsertedCount.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // _btnReviewApi
            // 
            _btnReviewApi.BackColor = Color.DodgerBlue;
            _btnReviewApi.ForeColor = Color.White;
            _btnReviewApi.Location = new Point(1000, 750);
            _btnReviewApi.Name = "_btnReviewApi";
            _btnReviewApi.Size = new Size(100, 40);
            _btnReviewApi.TabIndex = 16;
            _btnReviewApi.Text = "Review API";
            _btnReviewApi.UseVisualStyleBackColor = false;
            _btnReviewApi.Visible = false;
            _btnReviewApi.Click += _btnReviewApi_Click;
            // 
            // _btnFinalize
            // 
            _btnFinalize.BackColor = Color.LimeGreen;
            _btnFinalize.Location = new Point(1120, 750);
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
            _btnReset.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _btnReset.Location = new Point(698, 750);
            _btnReset.Name = "_btnReset";
            _btnReset.Size = new Size(151, 71);
            _btnReset.TabIndex = 8;
            _btnReset.Text = "Reset";
            _btnReset.UseVisualStyleBackColor = true;
            _btnReset.Click += _btnReset_Click;
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
            _lblApiInfo.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            _lblApiInfo.ForeColor = Color.FromArgb(20, 55, 100);
            _lblApiInfo.Location = new Point(31, 628);
            _lblApiInfo.Name = "_lblApiInfo";
            _lblApiInfo.Size = new Size(71, 21);
            _lblApiInfo.TabIndex = 17;
            _lblApiInfo.Text = "API Info";
            // 
            // _txtApiInfo
            // 
            _txtApiInfo.Font = new Font("Consolas", 9F);
            _txtApiInfo.Location = new Point(31, 652);
            _txtApiInfo.Multiline = true;
            _txtApiInfo.Name = "_txtApiInfo";
            _txtApiInfo.ReadOnly = true;
            _txtApiInfo.ScrollBars = ScrollBars.Vertical;
            _txtApiInfo.Size = new Size(661, 169);
            _txtApiInfo.TabIndex = 18;
            // 
            // panel1
            // 
            panel1.BackColor = Color.FromArgb(20, 55, 100);
            panel1.Controls.Add(button1);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(1343, 24);
            panel1.TabIndex = 12;
            // 
            // button1
            // 
            button1.Dock = DockStyle.Right;
            button1.FlatAppearance.BorderSize = 0;
            button1.FlatStyle = FlatStyle.Flat;
            button1.Image = (Image)resources.GetObject("button1.Image");
            button1.Location = new Point(1295, 0);
            button1.Name = "button1";
            button1.Size = new Size(48, 24);
            button1.TabIndex = 0;
            button1.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            panel2.BackColor = Color.FromArgb(20, 55, 100);
            panel2.Dock = DockStyle.Top;
            panel2.Location = new Point(0, 24);
            panel2.Name = "panel2";
            panel2.Size = new Size(1343, 51);
            panel2.TabIndex = 13;
            // 
            // lblInfo
            // 
            lblInfo.Dock = DockStyle.Top;
            lblInfo.Font = new Font("Malgun Gothic", 30F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblInfo.ForeColor = Color.DarkOrange;
            lblInfo.Location = new Point(0, 166);
            lblInfo.Name = "lblInfo";
            lblInfo.Size = new Size(1343, 97);
            lblInfo.TabIndex = 14;
            lblInfo.Text = "--";
            lblInfo.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblBG
            // 
            lblBG.Dock = DockStyle.Top;
            lblBG.Font = new Font("Malgun Gothic", 30F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblBG.ForeColor = Color.DarkOrange;
            lblBG.Location = new Point(0, 263);
            lblBG.Name = "lblBG";
            lblBG.Size = new Size(1343, 87);
            lblBG.TabIndex = 15;
            lblBG.Text = "--";
            lblBG.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // StationPanel
            // 
            BackColor = Color.White;
            Controls.Add(lblBG);
            Controls.Add(lblInfo);
            Controls.Add(_lblStation);
            Controls.Add(panel2);
            Controls.Add(panel1);
            Controls.Add(_txtScanInput);
            Controls.Add(_lstCables);
            Controls.Add(_lblCableCount);
            Controls.Add(_lblScrapCount);
            Controls.Add(_lblInsertedCount);
            Controls.Add(_txtApiInfo);
            Controls.Add(_lblApiInfo);
            Controls.Add(_btnReviewApi);
            Controls.Add(_btnFinalize);
            Controls.Add(_btnReset);
            Controls.Add(_lblStatus);
            Name = "StationPanel";
            Size = new Size(1343, 852);
            panel1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }
        private Panel panel1;
        private Button button1;
        private Panel panel2;
        private Label lblInfo;
        private Label lblBG;
    }
}
