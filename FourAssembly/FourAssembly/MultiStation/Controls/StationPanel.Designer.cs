namespace FourAssembly.MultiStation.Controls
{
    partial class StationPanel
    {
        private System.ComponentModel.IContainer components = null;
        private Label _lblStation;
        private Label _lblInstruction;
        private TextBox _txtScanInput;
        private ListView _lstCables;
        private Label _lblCableCount;
        private Button _btnFinalize;
        private Button _btnReset;
        private Label _lblStatus;

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
            _lblInstruction = new Label();
            _txtScanInput = new TextBox();
            _lstCables = new ListView();
            _lblCableCount = new Label();
            _btnFinalize = new Button();
            _btnReset = new Button();
            _lblStatus = new Label();
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
            // _lblInstruction
            // 
            _lblInstruction.AutoSize = true;
            _lblInstruction.Location = new Point(20, 60);
            _lblInstruction.MaximumSize = new Size(850, 0);
            _lblInstruction.Name = "_lblInstruction";
            _lblInstruction.Size = new Size(0, 15);
            _lblInstruction.TabIndex = 1;
            // 
            // _txtScanInput
            // 
            _txtScanInput.Font = new Font("Consolas", 11F);
            _txtScanInput.Location = new Point(23, 750);
            _txtScanInput.Name = "_txtScanInput";
            _txtScanInput.Size = new Size(450, 25);
            _txtScanInput.TabIndex = 3;
            _txtScanInput.KeyDown += _txtScanInput_KeyDown;
            // 
            // _lstCables
            // 
            _lstCables.Location = new Point(23, 324);
            _lstCables.Name = "_lstCables";
            _lstCables.Size = new Size(672, 350);
            _lstCables.TabIndex = 5;
            _lstCables.UseCompatibleStateImageBehavior = false;
            _lstCables.View = View.List;
            // 
            // _lblCableCount
            // 
            _lblCableCount.AutoSize = true;
            _lblCableCount.Font = new Font("Segoe UI", 20.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            _lblCableCount.Location = new Point(701, 637);
            _lblCableCount.Name = "_lblCableCount";
            _lblCableCount.Size = new Size(117, 37);
            _lblCableCount.TabIndex = 6;
            _lblCableCount.Text = "Count: 0";
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
            _btnFinalize.Click += _btnFinalize_Click;
            // 
            // _btnReset
            // 
            _btnReset.Location = new Point(1240, 750);
            _btnReset.Name = "_btnReset";
            _btnReset.Size = new Size(100, 40);
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
            lblInfo.Font = new Font("Malgun Gothic", 27.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblInfo.ForeColor = Color.DarkOrange;
            lblInfo.Location = new Point(0, 166);
            lblInfo.Name = "lblInfo";
            lblInfo.Size = new Size(1343, 76);
            lblInfo.TabIndex = 14;
            lblInfo.Text = "--";
            lblInfo.TextAlign = ContentAlignment.MiddleCenter;
            lblInfo.Visible = false;
            // 
            // lblBG
            // 
            lblBG.Dock = DockStyle.Top;
            lblBG.Font = new Font("Malgun Gothic", 27.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblBG.ForeColor = Color.FromArgb(20, 55, 100);
            lblBG.Location = new Point(0, 242);
            lblBG.Name = "lblBG";
            lblBG.Size = new Size(1343, 76);
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
            Controls.Add(_lblInstruction);
            Controls.Add(_txtScanInput);
            Controls.Add(_lstCables);
            Controls.Add(_lblCableCount);
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
