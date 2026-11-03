namespace FourAssembly.MultiStation
{
    partial class FrmMain
    {
        private System.ComponentModel.IContainer components = null;
        private Panel menuPanel;
        private Button btnEstaciones;
        private Button btnRecipeEditor;
        private Button btnCognex;
        private Button btnPlc;
        private Button btnPresence;
        private Button btnSettings;
        private Button btnCounters;
        private Button btnToggleSelector;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            menuPanel = new Panel();
            btnRecipeEditor = new Button();
            btnCounters = new Button();
            btnEstaciones = new Button();
            btnToggleSelector = new Button();
            btnCognex = new Button();
            btnPresence = new Button();
            btnPlc = new Button();
            btnSettings = new Button();
            pnMenu = new Panel();
            button1 = new Button();
            cmbVariante = new ComboBox();
            label1 = new Label();
            lblInfo = new Label();
            _txtScanInput = new TextBox();
            pnlMasterScan = new Panel();
            pnlBottomArea = new Panel();
            pnlCmb = new Panel();
            panel5 = new Panel();
            btnActive = new Button();
            panel4 = new Panel();
            panel2 = new Panel();
            menuPanel.SuspendLayout();
            pnMenu.SuspendLayout();
            pnlMasterScan.SuspendLayout();
            pnlBottomArea.SuspendLayout();
            pnlCmb.SuspendLayout();
            SuspendLayout();
            // 
            // menuPanel
            // 
            menuPanel.BackColor = Color.FromArgb(20, 55, 100);
            menuPanel.Controls.Add(btnRecipeEditor);
            menuPanel.Controls.Add(btnCounters);
            menuPanel.Controls.Add(btnEstaciones);
            menuPanel.Controls.Add(btnToggleSelector);
            menuPanel.Controls.Add(btnCognex);
            menuPanel.Controls.Add(btnPresence);
            menuPanel.Controls.Add(btnPlc);
            menuPanel.Controls.Add(btnSettings);
            menuPanel.Dock = DockStyle.Top;
            menuPanel.Location = new Point(0, 23);
            menuPanel.Name = "menuPanel";
            menuPanel.Size = new Size(1200, 44);
            menuPanel.TabIndex = 0;
            // 
            // btnRecipeEditor
            // 
            btnRecipeEditor.BackColor = Color.FromArgb(20, 55, 100);
            btnRecipeEditor.Dock = DockStyle.Right;
            btnRecipeEditor.Font = new Font("Segoe UI", 12F);
            btnRecipeEditor.ForeColor = Color.White;
            btnRecipeEditor.Location = new Point(491, 0);
            btnRecipeEditor.Name = "btnRecipeEditor";
            btnRecipeEditor.Size = new Size(103, 44);
            btnRecipeEditor.TabIndex = 1;
            btnRecipeEditor.Text = "Recetas";
            btnRecipeEditor.UseVisualStyleBackColor = false;
            btnRecipeEditor.Click += btnRecipeEditor_Click;
            // 
            // btnCounters
            // 
            btnCounters.BackColor = Color.FromArgb(20, 55, 100);
            btnCounters.Dock = DockStyle.Right;
            btnCounters.Font = new Font("Segoe UI", 12F);
            btnCounters.ForeColor = Color.White;
            btnCounters.Location = new Point(594, 0);
            btnCounters.Name = "btnCounters";
            btnCounters.Size = new Size(130, 44);
            btnCounters.TabIndex = 5;
            btnCounters.Text = "Contadores";
            btnCounters.UseVisualStyleBackColor = false;
            btnCounters.Click += btnCounters_Click;
            // 
            // btnEstaciones
            // 
            btnEstaciones.BackColor = Color.FromArgb(20, 55, 100);
            btnEstaciones.Dock = DockStyle.Left;
            btnEstaciones.Font = new Font("Segoe UI", 12F);
            btnEstaciones.ForeColor = Color.White;
            btnEstaciones.Location = new Point(160, 0);
            btnEstaciones.Name = "btnEstaciones";
            btnEstaciones.Size = new Size(183, 44);
            btnEstaciones.TabIndex = 0;
            btnEstaciones.Text = "Estaciones";
            btnEstaciones.UseVisualStyleBackColor = false;
            btnEstaciones.Click += btnEstaciones_Click;
            // 
            // btnToggleSelector
            // 
            btnToggleSelector.BackColor = Color.FromArgb(20, 55, 100);
            btnToggleSelector.Dock = DockStyle.Left;
            btnToggleSelector.Font = new Font("Segoe UI", 12F);
            btnToggleSelector.ForeColor = Color.White;
            btnToggleSelector.Location = new Point(0, 0);
            btnToggleSelector.Name = "btnToggleSelector";
            btnToggleSelector.Size = new Size(160, 44);
            btnToggleSelector.TabIndex = 6;
            btnToggleSelector.Text = "Ocultar selección";
            btnToggleSelector.UseVisualStyleBackColor = false;
            btnToggleSelector.Click += btnToggleSelector_Click;
            // 
            // btnCognex
            // 
            btnCognex.BackColor = Color.FromArgb(20, 55, 100);
            btnCognex.Dock = DockStyle.Right;
            btnCognex.Font = new Font("Segoe UI", 12F);
            btnCognex.ForeColor = Color.White;
            btnCognex.Location = new Point(724, 0);
            btnCognex.Name = "btnCognex";
            btnCognex.Size = new Size(148, 44);
            btnCognex.TabIndex = 2;
            btnCognex.Text = "Cognex Debug";
            btnCognex.UseVisualStyleBackColor = false;
            btnCognex.Click += btnCognex_Click;
            // 
            // btnPresence
            // 
            btnPresence.BackColor = Color.FromArgb(20, 55, 100);
            btnPresence.Dock = DockStyle.Right;
            btnPresence.Font = new Font("Segoe UI", 12F);
            btnPresence.ForeColor = Color.White;
            btnPresence.Location = new Point(872, 0);
            btnPresence.Name = "btnPresence";
            btnPresence.Size = new Size(128, 44);
            btnPresence.TabIndex = 8;
            btnPresence.Text = "Presencias";
            btnPresence.UseVisualStyleBackColor = false;
            btnPresence.Click += btnPresence_Click;
            // 
            // btnPlc
            // 
            btnPlc.BackColor = Color.FromArgb(20, 55, 100);
            btnPlc.Dock = DockStyle.Right;
            btnPlc.Font = new Font("Segoe UI", 12F);
            btnPlc.ForeColor = Color.White;
            btnPlc.Location = new Point(1000, 0);
            btnPlc.Name = "btnPlc";
            btnPlc.Size = new Size(112, 44);
            btnPlc.TabIndex = 4;
            btnPlc.Text = "PLC Debug";
            btnPlc.UseVisualStyleBackColor = false;
            btnPlc.Click += btnPlc_Click;
            // 
            // btnSettings
            // 
            btnSettings.BackColor = Color.FromArgb(20, 55, 100);
            btnSettings.Dock = DockStyle.Right;
            btnSettings.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnSettings.ForeColor = Color.White;
            btnSettings.Location = new Point(1112, 0);
            btnSettings.Name = "btnSettings";
            btnSettings.Size = new Size(88, 44);
            btnSettings.TabIndex = 3;
            btnSettings.Text = "Settings";
            btnSettings.UseVisualStyleBackColor = false;
            btnSettings.Click += btnSettings_Click;
            // 
            // pnMenu
            // 
            pnMenu.BackColor = Color.FromArgb(20, 55, 100);
            pnMenu.Controls.Add(button1);
            pnMenu.Dock = DockStyle.Top;
            pnMenu.Location = new Point(0, 0);
            pnMenu.Name = "pnMenu";
            pnMenu.Size = new Size(1200, 23);
            pnMenu.TabIndex = 1;
            pnMenu.MouseDown += pnMenu_MouseDown;
            // 
            // button1
            // 
            button1.Dock = DockStyle.Right;
            button1.FlatAppearance.BorderSize = 0;
            button1.FlatStyle = FlatStyle.Flat;
            button1.Image = (Image)resources.GetObject("button1.Image");
            button1.Location = new Point(1152, 0);
            button1.Name = "button1";
            button1.Size = new Size(48, 23);
            button1.TabIndex = 0;
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // cmbVariante
            // 
            cmbVariante.Dock = DockStyle.Fill;
            cmbVariante.Font = new Font("Segoe UI", 24F, FontStyle.Regular, GraphicsUnit.Point, 0);
            cmbVariante.FormattingEnabled = true;
            cmbVariante.Location = new Point(0, 0);
            cmbVariante.Name = "cmbVariante";
            cmbVariante.Size = new Size(834, 53);
            cmbVariante.TabIndex = 11;
            cmbVariante.SelectedIndexChanged += cmbVariante_SelectedIndexChanged;
            // 
            // label1
            // 
            label1.Dock = DockStyle.Top;
            label1.Font = new Font("Segoe UI", 30F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.FromArgb(20, 55, 100);
            label1.Location = new Point(0, 75);
            label1.Name = "label1";
            label1.Size = new Size(1200, 62);
            label1.TabIndex = 0;
            label1.Text = "Seleccionar número de parte";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblInfo
            // 
            lblInfo.Dock = DockStyle.Fill;
            lblInfo.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblInfo.Location = new Point(0, 0);
            lblInfo.Name = "lblInfo";
            lblInfo.Size = new Size(1200, 93);
            lblInfo.TabIndex = 12;
            lblInfo.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // _txtScanInput
            // 
            _txtScanInput.BackColor = Color.FromArgb(235, 242, 250);
            _txtScanInput.BorderStyle = BorderStyle.None;
            _txtScanInput.Dock = DockStyle.Fill;
            _txtScanInput.Font = new Font("Consolas", 12F);
            _txtScanInput.Location = new Point(20, 5);
            _txtScanInput.Margin = new Padding(0);
            _txtScanInput.Name = "_txtScanInput";
            _txtScanInput.Size = new Size(1160, 19);
            _txtScanInput.TabIndex = 13;
            _txtScanInput.Visible = false;
            // 
            // pnlMasterScan
            // 
            pnlMasterScan.BackColor = Color.FromArgb(235, 242, 250);
            pnlMasterScan.Controls.Add(_txtScanInput);
            pnlMasterScan.Dock = DockStyle.Bottom;
            pnlMasterScan.Location = new Point(0, 93);
            pnlMasterScan.Name = "pnlMasterScan";
            pnlMasterScan.Padding = new Padding(20, 5, 20, 5);
            pnlMasterScan.Size = new Size(1200, 30);
            pnlMasterScan.TabIndex = 15;
            // 
            // pnlBottomArea
            // 
            pnlBottomArea.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pnlBottomArea.BackColor = Color.White;
            pnlBottomArea.Controls.Add(lblInfo);
            pnlBottomArea.Controls.Add(pnlMasterScan);
            pnlBottomArea.Location = new Point(0, 547);
            pnlBottomArea.Name = "pnlBottomArea";
            pnlBottomArea.Size = new Size(1200, 123);
            pnlBottomArea.TabIndex = 16;
            // 
            // pnlCmb
            // 
            pnlCmb.Controls.Add(cmbVariante);
            pnlCmb.Controls.Add(panel5);
            pnlCmb.Controls.Add(btnActive);
            pnlCmb.Controls.Add(panel4);
            pnlCmb.Dock = DockStyle.Top;
            pnlCmb.Font = new Font("Segoe UI", 20F);
            pnlCmb.Location = new Point(0, 137);
            pnlCmb.Name = "pnlCmb";
            pnlCmb.Size = new Size(1200, 54);
            pnlCmb.TabIndex = 14;
            // 
            // panel5
            // 
            panel5.Dock = DockStyle.Right;
            panel5.Location = new Point(834, 0);
            panel5.Name = "panel5";
            panel5.Size = new Size(35, 54);
            panel5.TabIndex = 6;
            // 
            // btnActive
            // 
            btnActive.BackColor = Color.FromArgb(221, 236, 255);
            btnActive.Dock = DockStyle.Right;
            btnActive.Font = new Font("Candara", 20F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnActive.ForeColor = Color.FromArgb(20, 55, 100);
            btnActive.Location = new Point(869, 0);
            btnActive.Name = "btnActive";
            btnActive.Size = new Size(283, 54);
            btnActive.TabIndex = 4;
            btnActive.Text = "Activar variante";
            btnActive.UseVisualStyleBackColor = false;
            btnActive.Click += btnActive_Click;
            // 
            // panel4
            // 
            panel4.Dock = DockStyle.Right;
            panel4.Location = new Point(1152, 0);
            panel4.Name = "panel4";
            panel4.Size = new Size(48, 54);
            panel4.TabIndex = 5;
            // 
            // panel2
            // 
            panel2.BackColor = Color.FromArgb(20, 55, 100);
            panel2.Dock = DockStyle.Top;
            panel2.Location = new Point(0, 67);
            panel2.Name = "panel2";
            panel2.Size = new Size(1200, 8);
            panel2.TabIndex = 2;
            // 
            // FrmMain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(1200, 700);
            Controls.Add(pnlCmb);
            Controls.Add(pnlBottomArea);
            Controls.Add(label1);
            Controls.Add(panel2);
            Controls.Add(menuPanel);
            Controls.Add(pnMenu);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FrmMain";
            menuPanel.ResumeLayout(false);
            pnMenu.ResumeLayout(false);
            pnlMasterScan.ResumeLayout(false);
            pnlMasterScan.PerformLayout();
            pnlBottomArea.ResumeLayout(false);
            pnlCmb.ResumeLayout(false);
            ResumeLayout(false);
        }
        private Panel pnMenu;
        private Button button1;
        private ComboBox cmbVariante;
        private Label label1;
        private Label lblInfo;
        private TextBox _txtScanInput;
        private Panel pnlCmb;
        private Button btnActive;
        private Panel panel4;
        private Panel panel5;
        private Panel panel2;
        private Panel pnlMasterScan;
        private Panel pnlBottomArea;
    }
}
