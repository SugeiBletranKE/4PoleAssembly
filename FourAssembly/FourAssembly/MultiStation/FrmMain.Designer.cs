namespace FourAssembly.MultiStation
{
    partial class FrmMain
    {
        private System.ComponentModel.IContainer components = null;
        private Panel menuPanel;
        private Button btnEstaciones;
        private Button btnRecipeEditor;
        private Button btnCognex;
        private Button btnSettings;

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
            btnEstaciones = new Button();
            btnRecipeEditor = new Button();
            btnCognex = new Button();
            btnSettings = new Button();
            panel1 = new Panel();
            button1 = new Button();
            panel2 = new Panel();
            panel3 = new Panel();
            cmbVariante = new ComboBox();
            label1 = new Label();
            menuPanel.SuspendLayout();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // menuPanel
            // 
            menuPanel.BackColor = Color.FromArgb(20, 55, 100);
            menuPanel.Controls.Add(btnEstaciones);
            menuPanel.Controls.Add(btnRecipeEditor);
            menuPanel.Controls.Add(btnCognex);
            menuPanel.Controls.Add(btnSettings);
            menuPanel.Dock = DockStyle.Top;
            menuPanel.Location = new Point(0, 30);
            menuPanel.Name = "menuPanel";
            menuPanel.Size = new Size(1200, 44);
            menuPanel.TabIndex = 0;
            // 
            // btnEstaciones
            // 
            btnEstaciones.BackColor = Color.FromArgb(20, 55, 100);
            btnEstaciones.Dock = DockStyle.Left;
            btnEstaciones.Font = new Font("Segoe UI", 12F);
            btnEstaciones.ForeColor = Color.White;
            btnEstaciones.Location = new Point(148, 0);
            btnEstaciones.Name = "btnEstaciones";
            btnEstaciones.Size = new Size(148, 44);
            btnEstaciones.TabIndex = 0;
            btnEstaciones.Text = "Estaciones";
            btnEstaciones.UseVisualStyleBackColor = false;
            btnEstaciones.Click += btnEstaciones_Click;
            // 
            // btnRecipeEditor
            // 
            btnRecipeEditor.BackColor = Color.FromArgb(20, 55, 100);
            btnRecipeEditor.Dock = DockStyle.Left;
            btnRecipeEditor.Font = new Font("Segoe UI", 12F);
            btnRecipeEditor.ForeColor = Color.White;
            btnRecipeEditor.Location = new Point(0, 0);
            btnRecipeEditor.Name = "btnRecipeEditor";
            btnRecipeEditor.Size = new Size(148, 44);
            btnRecipeEditor.TabIndex = 1;
            btnRecipeEditor.Text = "Recetas";
            btnRecipeEditor.UseVisualStyleBackColor = false;
            btnRecipeEditor.Click += btnRecipeEditor_Click;
            // 
            // btnCognex
            // 
            btnCognex.BackColor = Color.FromArgb(20, 55, 100);
            btnCognex.Dock = DockStyle.Right;
            btnCognex.Font = new Font("Segoe UI", 12F);
            btnCognex.ForeColor = Color.White;
            btnCognex.Location = new Point(904, 0);
            btnCognex.Name = "btnCognex";
            btnCognex.Size = new Size(148, 44);
            btnCognex.TabIndex = 2;
            btnCognex.Text = "Cognex Debug";
            btnCognex.UseVisualStyleBackColor = false;
            btnCognex.Click += btnCognex_Click;
            // 
            // btnSettings
            // 
            btnSettings.BackColor = Color.FromArgb(20, 55, 100);
            btnSettings.Dock = DockStyle.Right;
            btnSettings.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnSettings.ForeColor = Color.White;
            btnSettings.Location = new Point(1052, 0);
            btnSettings.Name = "btnSettings";
            btnSettings.Size = new Size(148, 44);
            btnSettings.TabIndex = 3;
            btnSettings.Text = "Settings";
            btnSettings.UseVisualStyleBackColor = false;
            btnSettings.Click += btnSettings_Click;
            // 
            // panel1
            // 
            panel1.BackColor = Color.FromArgb(20, 55, 100);
            panel1.Controls.Add(button1);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(1200, 30);
            panel1.TabIndex = 1;
            // 
            // button1
            // 
            button1.Dock = DockStyle.Right;
            button1.FlatAppearance.BorderSize = 0;
            button1.FlatStyle = FlatStyle.Flat;
            button1.Image = (Image)resources.GetObject("button1.Image");
            button1.Location = new Point(1152, 0);
            button1.Name = "button1";
            button1.Size = new Size(48, 30);
            button1.TabIndex = 0;
            button1.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            panel2.BackColor = Color.FromArgb(20, 55, 100);
            panel2.Controls.Add(panel3);
            panel2.Dock = DockStyle.Top;
            panel2.Location = new Point(0, 74);
            panel2.Name = "panel2";
            panel2.Size = new Size(1200, 14);
            panel2.TabIndex = 2;
            // 
            // panel3
            // 
            panel3.BackColor = Color.FromArgb(20, 55, 100);
            panel3.Dock = DockStyle.Top;
            panel3.Location = new Point(0, 0);
            panel3.Name = "panel3";
            panel3.Size = new Size(1200, 14);
            panel3.TabIndex = 3;
            // 
            // cmbVariante
            // 
            cmbVariante.Dock = DockStyle.Top;
            cmbVariante.Font = new Font("Segoe UI", 24F, FontStyle.Regular, GraphicsUnit.Point, 0);
            cmbVariante.FormattingEnabled = true;
            cmbVariante.Location = new Point(0, 171);
            cmbVariante.Name = "cmbVariante";
            cmbVariante.Size = new Size(1200, 53);
            cmbVariante.TabIndex = 11;
            cmbVariante.SelectedIndexChanged += cmbVariante_SelectedIndexChanged;
            // 
            // label1
            //
            label1.Dock = DockStyle.Top;
            label1.Font = new Font("Segoe UI", 27.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.FromArgb(20, 55, 100);
            label1.Location = new Point(0, 88);
            label1.Name = "label1";
            label1.Size = new Size(1200, 83);
            label1.TabIndex = 0;
            label1.Text = "Seleccionar n�mero de parte";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            //
            // lblInfo
            //
            lblInfo = new Label();
            lblInfo.Dock = DockStyle.Bottom;
            lblInfo.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblInfo.Location = new Point(0, 600);
            lblInfo.Name = "lblInfo";
            lblInfo.Size = new Size(1200, 100);
            lblInfo.TabIndex = 12;
            lblInfo.Text = "";
            lblInfo.TextAlign = ContentAlignment.MiddleCenter;
            //
            // _txtScanInput
            //
            _txtScanInput = new TextBox();
            _txtScanInput.Location = new Point(50, 650);
            _txtScanInput.Name = "_txtScanInput";
            _txtScanInput.Size = new Size(1100, 30);
            _txtScanInput.Font = new Font("Consolas", 12F);
            _txtScanInput.TabIndex = 13;
            _txtScanInput.Visible = false;
            //
            // FrmMain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(1200, 700);
            Controls.Add(_txtScanInput);
            Controls.Add(lblInfo);
            Controls.Add(cmbVariante);
            Controls.Add(label1);
            Controls.Add(panel2);
            Controls.Add(menuPanel);
            Controls.Add(panel1);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FrmMain";
            Text = "FourAssembly - Multi-Station Control";
            menuPanel.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel2.ResumeLayout(false);
            ResumeLayout(false);
        }
        private Panel panel1;
        private Button button1;
        private Panel panel2;
        private Panel panel3;
        private ComboBox cmbVariante;
        private Label label1;
        private Label lblInfo;
        private TextBox _txtScanInput;
    }
}
