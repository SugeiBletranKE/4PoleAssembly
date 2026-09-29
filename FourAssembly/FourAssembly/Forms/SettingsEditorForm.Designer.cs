namespace FourAssembly.Forms;

partial class SettingsEditorForm
{
    private System.ComponentModel.IContainer components = null;
    private Panel pnlHeader;
    private Label lblTitle;
    private Label lblDescription;
    private Label lblPathCaption;
    private Label lblPath;
    private TextBox txtJson;
    private Panel pnlBottom;
    private Label lblStatus;
    private Button btnReload;
    private Button btnFormat;
    private Button btnSave;
    private Button btnSaveAndRestart;
    private Button btnClose;
    private Panel pnlResets;
    private Label lblResets;
    private Button btnResetGeneral;
    private Button btnResetStation1;
    private Button btnResetStation2;
    private Button btnResetStation3;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null)
            components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        pnlHeader = new Panel();
        lblTitle = new Label();
        lblDescription = new Label();
        lblPathCaption = new Label();
        lblPath = new Label();
        txtJson = new TextBox();
        pnlBottom = new Panel();
        lblStatus = new Label();
        btnReload = new Button();
        btnFormat = new Button();
        btnSave = new Button();
        btnSaveAndRestart = new Button();
        btnClose = new Button();
        pnlResets = new Panel();
        lblResets = new Label();
        btnResetGeneral = new Button();
        btnResetStation1 = new Button();
        btnResetStation2 = new Button();
        btnResetStation3 = new Button();
        pnlHeader.SuspendLayout();
        pnlBottom.SuspendLayout();
        pnlResets.SuspendLayout();
        SuspendLayout();
        // 
        // pnlHeader
        // 
        pnlHeader.BackColor = Color.FromArgb(20, 55, 100);
        pnlHeader.Controls.Add(lblTitle);
        pnlHeader.Controls.Add(lblDescription);
        pnlHeader.Controls.Add(lblPathCaption);
        pnlHeader.Controls.Add(lblPath);
        pnlHeader.Dock = DockStyle.Top;
        pnlHeader.Location = new Point(0, 0);
        pnlHeader.Name = "pnlHeader";
        pnlHeader.Size = new Size(1000, 105);
        pnlHeader.TabIndex = 0;
        // 
        // lblTitle
        // 
        lblTitle.AutoSize = true;
        lblTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
        lblTitle.ForeColor = Color.White;
        lblTitle.Location = new Point(18, 12);
        lblTitle.Name = "lblTitle";
        lblTitle.Size = new Size(259, 32);
        lblTitle.TabIndex = 0;
        lblTitle.Text = "Editor de configuracion";
        // 
        // lblDescription
        // 
        lblDescription.AutoSize = true;
        lblDescription.ForeColor = Color.WhiteSmoke;
        lblDescription.Location = new Point(21, 48);
        lblDescription.Name = "lblDescription";
        lblDescription.Size = new Size(583, 15);
        lblDescription.TabIndex = 1;
        lblDescription.Text = "Edita settings.json como texto. El archivo se valida antes de guardar y los cambios requieren reiniciar.";
        // 
        // lblPathCaption
        // 
        lblPathCaption.AutoSize = true;
        lblPathCaption.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblPathCaption.ForeColor = Color.White;
        lblPathCaption.Location = new Point(21, 76);
        lblPathCaption.Name = "lblPathCaption";
        lblPathCaption.Size = new Size(51, 15);
        lblPathCaption.TabIndex = 2;
        lblPathCaption.Text = "Archivo:";
        // 
        // lblPath
        // 
        lblPath.AutoEllipsis = true;
        lblPath.ForeColor = Color.White;
        lblPath.Location = new Point(77, 76);
        lblPath.Name = "lblPath";
        lblPath.Size = new Size(900, 18);
        lblPath.TabIndex = 3;
        lblPath.Text = "settings.json";
        // 
        // txtJson
        // 
        txtJson.AcceptsReturn = true;
        txtJson.AcceptsTab = true;
        txtJson.BackColor = Color.FromArgb(30, 30, 30);
        txtJson.Dock = DockStyle.Fill;
        txtJson.Font = new Font("Consolas", 11F);
        txtJson.ForeColor = Color.Gainsboro;
        txtJson.Location = new Point(0, 105);
        txtJson.MaxLength = 0;
        txtJson.Multiline = true;
        txtJson.Name = "txtJson";
        txtJson.ScrollBars = ScrollBars.Both;
        txtJson.Size = new Size(1000, 579);
        txtJson.TabIndex = 1;
        txtJson.WordWrap = false;
        // 
        // pnlBottom
        // 
        pnlBottom.BackColor = Color.WhiteSmoke;
        pnlBottom.Controls.Add(lblStatus);
        pnlBottom.Controls.Add(btnReload);
        pnlBottom.Controls.Add(btnFormat);
        pnlBottom.Controls.Add(btnSave);
        pnlBottom.Controls.Add(btnSaveAndRestart);
        pnlBottom.Controls.Add(btnClose);
        pnlBottom.Dock = DockStyle.Bottom;
        pnlBottom.Location = new Point(0, 684);
        pnlBottom.Name = "pnlBottom";
        pnlBottom.Size = new Size(1000, 66);
        pnlBottom.TabIndex = 2;
        // 
        // lblStatus
        // 
        lblStatus.AutoEllipsis = true;
        lblStatus.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblStatus.Location = new Point(17, 12);
        lblStatus.Name = "lblStatus";
        lblStatus.Size = new Size(426, 42);
        lblStatus.TabIndex = 0;
        lblStatus.Text = "Listo.";
        lblStatus.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // btnReload
        // 
        btnReload.Location = new Point(449, 14);
        btnReload.Name = "btnReload";
        btnReload.Size = new Size(100, 38);
        btnReload.TabIndex = 1;
        btnReload.Text = "Recargar";
        btnReload.UseVisualStyleBackColor = true;
        btnReload.Click += BtnReload_Click;
        // 
        // btnFormat
        // 
        btnFormat.Location = new Point(555, 14);
        btnFormat.Name = "btnFormat";
        btnFormat.Size = new Size(100, 38);
        btnFormat.TabIndex = 2;
        btnFormat.Text = "Formatear";
        btnFormat.UseVisualStyleBackColor = true;
        btnFormat.Click += BtnFormat_Click;
        // 
        // btnSave
        // 
        btnSave.BackColor = Color.SeaGreen;
        btnSave.FlatStyle = FlatStyle.Flat;
        btnSave.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnSave.ForeColor = Color.White;
        btnSave.Location = new Point(661, 14);
        btnSave.Name = "btnSave";
        btnSave.Size = new Size(115, 38);
        btnSave.TabIndex = 3;
        btnSave.Text = "Guardar";
        btnSave.UseVisualStyleBackColor = false;
        btnSave.Click += BtnSave_Click;
        // 
        // btnSaveAndRestart
        // 
        btnSaveAndRestart.BackColor = Color.DarkOrange;
        btnSaveAndRestart.FlatStyle = FlatStyle.Flat;
        btnSaveAndRestart.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnSaveAndRestart.ForeColor = Color.White;
        btnSaveAndRestart.Location = new Point(782, 14);
        btnSaveAndRestart.Name = "btnSaveAndRestart";
        btnSaveAndRestart.Size = new Size(127, 38);
        btnSaveAndRestart.TabIndex = 4;
        btnSaveAndRestart.Text = "Guardar y reiniciar";
        btnSaveAndRestart.UseVisualStyleBackColor = false;
        btnSaveAndRestart.Click += BtnSaveAndRestart_Click;
        // 
        // btnClose
        // 
        btnClose.DialogResult = DialogResult.Cancel;
        btnClose.Location = new Point(915, 14);
        btnClose.Name = "btnClose";
        btnClose.Size = new Size(66, 38);
        btnClose.TabIndex = 5;
        btnClose.Text = "Cerrar";
        btnClose.UseVisualStyleBackColor = true;
        // 
        // pnlResets
        // 
        pnlResets.BackColor = Color.FromArgb(235, 238, 242);
        pnlResets.Controls.Add(lblResets);
        pnlResets.Controls.Add(btnResetGeneral);
        pnlResets.Controls.Add(btnResetStation1);
        pnlResets.Controls.Add(btnResetStation2);
        pnlResets.Controls.Add(btnResetStation3);
        pnlResets.Dock = DockStyle.Bottom;
        pnlResets.Location = new Point(0, 624);
        pnlResets.Name = "pnlResets";
        pnlResets.Size = new Size(1000, 60);
        pnlResets.TabIndex = 3;
        // 
        // lblResets
        // 
        lblResets.AutoSize = true;
        lblResets.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblResets.Location = new Point(17, 20);
        lblResets.Name = "lblResets";
        lblResets.Size = new Size(174, 19);
        lblResets.Text = "Mantenimiento / Reset:";
        // 
        // btnResetGeneral
        // 
        btnResetGeneral.Location = new Point(210, 11);
        btnResetGeneral.Name = "btnResetGeneral";
        btnResetGeneral.Size = new Size(130, 38);
        btnResetGeneral.Text = "Reset general";
        btnResetGeneral.UseVisualStyleBackColor = true;
        btnResetGeneral.Click += BtnResetGeneral_Click;
        // 
        // btnResetStation1
        // 
        btnResetStation1.Location = new Point(350, 11);
        btnResetStation1.Name = "btnResetStation1";
        btnResetStation1.Size = new Size(120, 38);
        btnResetStation1.Text = "Reset estación 1";
        btnResetStation1.UseVisualStyleBackColor = true;
        btnResetStation1.Click += BtnResetStation1_Click;
        // 
        // btnResetStation2
        // 
        btnResetStation2.Location = new Point(480, 11);
        btnResetStation2.Name = "btnResetStation2";
        btnResetStation2.Size = new Size(120, 38);
        btnResetStation2.Text = "Reset estación 2";
        btnResetStation2.UseVisualStyleBackColor = true;
        btnResetStation2.Click += BtnResetStation2_Click;
        // 
        // btnResetStation3
        // 
        btnResetStation3.Location = new Point(610, 11);
        btnResetStation3.Name = "btnResetStation3";
        btnResetStation3.Size = new Size(120, 38);
        btnResetStation3.Text = "Reset estación 3";
        btnResetStation3.UseVisualStyleBackColor = true;
        btnResetStation3.Click += BtnResetStation3_Click;
        // 
        // SettingsEditorForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnClose;
        ClientSize = new Size(1000, 750);
        Controls.Add(txtJson);
        Controls.Add(pnlResets);
        Controls.Add(pnlBottom);
        Controls.Add(pnlHeader);
        MinimumSize = new Size(800, 600);
        Name = "SettingsEditorForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Settings";
        WindowState = FormWindowState.Maximized;
        pnlHeader.ResumeLayout(false);
        pnlHeader.PerformLayout();
        pnlBottom.ResumeLayout(false);
        pnlResets.ResumeLayout(false);
        pnlResets.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }
}
