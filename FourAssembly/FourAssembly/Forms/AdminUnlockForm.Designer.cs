namespace FourAssembly.Forms;

partial class AdminUnlockForm
{
    private System.ComponentModel.IContainer components = null;
    private Label lblTitle;
    private Label lblError;
    private Label lblPassword;
    private Label lblScanInstruction;
    private TextBox txtPassword;
    private Label lblStatus;
    private Button btnUnlock;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            components?.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        lblTitle = new Label();
        lblError = new Label();
        lblPassword = new Label();
        lblScanInstruction = new Label();
        txtPassword = new TextBox();
        lblStatus = new Label();
        btnUnlock = new Button();
        SuspendLayout();
        // 
        // lblTitle
        // 
        lblTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
        lblTitle.ForeColor = Color.Red;
        lblTitle.Location = new Point(20, 18);
        lblTitle.Name = "lblTitle";
        lblTitle.Size = new Size(480, 35);
        lblTitle.TabIndex = 0;
        lblTitle.Text = "ESTACION BLOQUEADA";
        lblTitle.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // lblError
        // 
        lblError.Font = new Font("Segoe UI", 11F);
        lblError.Location = new Point(20, 62);
        lblError.Name = "lblError";
        lblError.Size = new Size(480, 55);
        lblError.TabIndex = 1;
        lblError.Text = "Descripcion del error";
        lblError.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // lblPassword
        // 
        lblPassword.AutoSize = true;
        lblPassword.Location = new Point(45, 157);
        lblPassword.Name = "lblPassword";
        lblPassword.Size = new Size(177, 15);
        lblPassword.TabIndex = 2;
        lblPassword.Text = "Código de autorización:";
        // 
        // lblScanInstruction
        // 
        lblScanInstruction.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        lblScanInstruction.ForeColor = Color.FromArgb(20, 55, 100);
        lblScanInstruction.Location = new Point(45, 124);
        lblScanInstruction.Name = "lblScanInstruction";
        lblScanInstruction.Size = new Size(420, 25);
        lblScanInstruction.TabIndex = 2;
        lblScanInstruction.Text = "Escanee el QR de autorización con el escáner verde";
        lblScanInstruction.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // txtPassword
        // 
        txtPassword.Location = new Point(245, 153);
        txtPassword.Name = "txtPassword";
        txtPassword.Size = new Size(220, 23);
        txtPassword.TabIndex = 3;
        txtPassword.UseSystemPasswordChar = true;
        txtPassword.KeyDown += TxtPassword_KeyDown;
        // 
        // lblStatus
        // 
        lblStatus.ForeColor = Color.Red;
        lblStatus.Location = new Point(45, 185);
        lblStatus.Name = "lblStatus";
        lblStatus.Size = new Size(420, 24);
        lblStatus.TabIndex = 4;
        // 
        // btnUnlock
        // 
        btnUnlock.Location = new Point(208, 222);
        btnUnlock.Name = "btnUnlock";
        btnUnlock.Size = new Size(105, 35);
        btnUnlock.TabIndex = 5;
        btnUnlock.Text = "Desbloquear";
        btnUnlock.UseVisualStyleBackColor = true;
        btnUnlock.Click += BtnUnlock_Click;
        // 
        // AdminUnlockForm
        // 
        AcceptButton = btnUnlock;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(520, 287);
        Controls.Add(btnUnlock);
        Controls.Add(lblStatus);
        Controls.Add(txtPassword);
        Controls.Add(lblPassword);
        Controls.Add(lblScanInstruction);
        Controls.Add(lblError);
        Controls.Add(lblTitle);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "AdminUnlockForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Autorizacion de administrador";
        Activated += AdminUnlockForm_Activated;
        Shown += AdminUnlockForm_Shown;
        ResumeLayout(false);
        PerformLayout();
    }
}
