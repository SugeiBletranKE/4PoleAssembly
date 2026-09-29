namespace FourAssembly.Forms;

partial class StationErrorAlertForm
{
    private System.ComponentModel.IContainer components = null;
    private Panel pnlHeader;
    private Label lblWarningIcon;
    private Label lblTitle;
    private Label lblStation;
    private Label lblErrorTypeCaption;
    private Label lblErrorType;
    private GroupBox grpDetection;
    private Label lblExpectedCaption;
    private Label lblExpected;
    private Label lblDetectedCaption;
    private Label lblDetected;
    private Label lblSideCaption;
    private Label lblSide;
    private Label lblErrorCodeCaption;
    private Label lblErrorCode;
    private GroupBox grpInstructions;
    private Label lblInstructions;
    private Panel pnlStatus;
    private Label lblStatusCaption;
    private Label lblStatus;
    private Button btnAuthorize;
    private Button btnAcknowledge;
    private Button btnClose;
    private Label lblAuthorization;
    private TextBox txtAuthorization;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null)
            components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        pnlHeader = new Panel();
        lblWarningIcon = new Label();
        lblTitle = new Label();
        lblStation = new Label();
        lblErrorTypeCaption = new Label();
        lblErrorType = new Label();
        grpDetection = new GroupBox();
        lblExpectedCaption = new Label();
        lblExpected = new Label();
        lblDetectedCaption = new Label();
        lblDetected = new Label();
        lblSideCaption = new Label();
        lblSide = new Label();
        lblErrorCodeCaption = new Label();
        lblErrorCode = new Label();
        grpInstructions = new GroupBox();
        lblInstructions = new Label();
        pnlStatus = new Panel();
        lblStatusCaption = new Label();
        lblStatus = new Label();
        btnAuthorize = new Button();
        btnAcknowledge = new Button();
        btnClose = new Button();
        lblAuthorization = new Label();
        txtAuthorization = new TextBox();
        pnlHeader.SuspendLayout();
        grpDetection.SuspendLayout();
        grpInstructions.SuspendLayout();
        pnlStatus.SuspendLayout();
        SuspendLayout();
        // 
        // pnlHeader
        // 
        pnlHeader.BackColor = Color.Firebrick;
        pnlHeader.Controls.Add(lblWarningIcon);
        pnlHeader.Controls.Add(lblTitle);
        pnlHeader.Controls.Add(lblStation);
        pnlHeader.Dock = DockStyle.Top;
        pnlHeader.Location = new Point(0, 0);
        pnlHeader.Name = "pnlHeader";
        pnlHeader.Size = new Size(760, 112);
        pnlHeader.TabIndex = 0;
        // 
        // lblWarningIcon
        // 
        lblWarningIcon.Font = new Font("Segoe UI Symbol", 34F, FontStyle.Bold);
        lblWarningIcon.ForeColor = Color.White;
        lblWarningIcon.Location = new Point(22, 18);
        lblWarningIcon.Name = "lblWarningIcon";
        lblWarningIcon.Size = new Size(78, 70);
        lblWarningIcon.TabIndex = 0;
        lblWarningIcon.Text = "!";
        lblWarningIcon.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // lblTitle
        // 
        lblTitle.Font = new Font("Segoe UI", 22F, FontStyle.Bold);
        lblTitle.ForeColor = Color.White;
        lblTitle.Location = new Point(106, 17);
        lblTitle.Name = "lblTitle";
        lblTitle.Size = new Size(625, 47);
        lblTitle.TabIndex = 1;
        lblTitle.Text = "ESTACION BLOQUEADA";
        lblTitle.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // lblStation
        // 
        lblStation.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
        lblStation.ForeColor = Color.White;
        lblStation.Location = new Point(109, 66);
        lblStation.Name = "lblStation";
        lblStation.Size = new Size(620, 29);
        lblStation.TabIndex = 2;
        lblStation.Text = "Estacion 1";
        // 
        // lblErrorTypeCaption
        // 
        lblErrorTypeCaption.AutoSize = true;
        lblErrorTypeCaption.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblErrorTypeCaption.ForeColor = Color.DimGray;
        lblErrorTypeCaption.Location = new Point(27, 132);
        lblErrorTypeCaption.Name = "lblErrorTypeCaption";
        lblErrorTypeCaption.Size = new Size(137, 19);
        lblErrorTypeCaption.TabIndex = 1;
        lblErrorTypeCaption.Text = "ERROR DETECTADO";
        // 
        // lblErrorType
        // 
        lblErrorType.Font = new Font("Segoe UI", 17F, FontStyle.Bold);
        lblErrorType.ForeColor = Color.Firebrick;
        lblErrorType.Location = new Point(23, 153);
        lblErrorType.Name = "lblErrorType";
        lblErrorType.Size = new Size(709, 112);
        lblErrorType.TabIndex = 2;
        lblErrorType.Text = "PIN INCORRECTO";
        lblErrorType.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // grpDetection
        // 
        grpDetection.Controls.Add(lblExpectedCaption);
        grpDetection.Controls.Add(lblExpected);
        grpDetection.Controls.Add(lblDetectedCaption);
        grpDetection.Controls.Add(lblDetected);
        grpDetection.Controls.Add(lblSideCaption);
        grpDetection.Controls.Add(lblSide);
        grpDetection.Controls.Add(lblErrorCodeCaption);
        grpDetection.Controls.Add(lblErrorCode);
        grpDetection.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        grpDetection.Location = new Point(27, 211);
        grpDetection.Name = "grpDetection";
        grpDetection.Size = new Size(705, 142);
        grpDetection.TabIndex = 3;
        grpDetection.TabStop = false;
        grpDetection.Text = "Detalle de la deteccion";
        grpDetection.Visible = false;
        // 
        // lblExpectedCaption
        // 
        lblExpectedCaption.AutoSize = true;
        lblExpectedCaption.Font = new Font("Segoe UI", 10F);
        lblExpectedCaption.ForeColor = Color.DimGray;
        lblExpectedCaption.Location = new Point(22, 31);
        lblExpectedCaption.Name = "lblExpectedCaption";
        lblExpectedCaption.Size = new Size(68, 19);
        lblExpectedCaption.TabIndex = 0;
        lblExpectedCaption.Text = "Esperado:";
        // 
        // lblExpected
        // 
        lblExpected.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
        lblExpected.ForeColor = Color.DarkGreen;
        lblExpected.Location = new Point(22, 51);
        lblExpected.Name = "lblExpected";
        lblExpected.Size = new Size(286, 31);
        lblExpected.TabIndex = 1;
        lblExpected.Text = "Pin 2";
        // 
        // lblDetectedCaption
        // 
        lblDetectedCaption.AutoSize = true;
        lblDetectedCaption.Font = new Font("Segoe UI", 10F);
        lblDetectedCaption.ForeColor = Color.DimGray;
        lblDetectedCaption.Location = new Point(354, 31);
        lblDetectedCaption.Name = "lblDetectedCaption";
        lblDetectedCaption.Size = new Size(75, 19);
        lblDetectedCaption.TabIndex = 2;
        lblDetectedCaption.Text = "Detectado:";
        // 
        // lblDetected
        // 
        lblDetected.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
        lblDetected.ForeColor = Color.Firebrick;
        lblDetected.Location = new Point(354, 51);
        lblDetected.Name = "lblDetected";
        lblDetected.Size = new Size(322, 31);
        lblDetected.TabIndex = 3;
        lblDetected.Text = "Pin 3";
        // 
        // lblSideCaption
        // 
        lblSideCaption.AutoSize = true;
        lblSideCaption.Font = new Font("Segoe UI", 10F);
        lblSideCaption.ForeColor = Color.DimGray;
        lblSideCaption.Location = new Point(22, 93);
        lblSideCaption.Name = "lblSideCaption";
        lblSideCaption.Size = new Size(42, 19);
        lblSideCaption.TabIndex = 4;
        lblSideCaption.Text = "Lado:";
        // 
        // lblSide
        // 
        lblSide.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblSide.Location = new Point(70, 93);
        lblSide.Name = "lblSide";
        lblSide.Size = new Size(238, 23);
        lblSide.TabIndex = 5;
        lblSide.Text = "Hembra / A";
        // 
        // lblErrorCodeCaption
        // 
        lblErrorCodeCaption.AutoSize = true;
        lblErrorCodeCaption.Font = new Font("Segoe UI", 10F);
        lblErrorCodeCaption.ForeColor = Color.DimGray;
        lblErrorCodeCaption.Location = new Point(354, 93);
        lblErrorCodeCaption.Name = "lblErrorCodeCaption";
        lblErrorCodeCaption.Size = new Size(56, 19);
        lblErrorCodeCaption.TabIndex = 6;
        lblErrorCodeCaption.Text = "Codigo:";
        // 
        // lblErrorCode
        // 
        lblErrorCode.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblErrorCode.Location = new Point(414, 93);
        lblErrorCode.Name = "lblErrorCode";
        lblErrorCode.Size = new Size(262, 23);
        lblErrorCode.TabIndex = 7;
        lblErrorCode.Text = "PIN_WRONG";
        // 
        // grpInstructions
        // 
        grpInstructions.Controls.Add(lblInstructions);
        grpInstructions.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        grpInstructions.Location = new Point(27, 367);
        grpInstructions.Name = "grpInstructions";
        grpInstructions.Size = new Size(705, 112);
        grpInstructions.TabIndex = 4;
        grpInstructions.TabStop = false;
        grpInstructions.Text = "Que debe hacer la operadora";
        grpInstructions.Visible = false;
        // 
        // lblInstructions
        // 
        lblInstructions.Font = new Font("Segoe UI", 11F);
        lblInstructions.Location = new Point(19, 26);
        lblInstructions.Name = "lblInstructions";
        lblInstructions.Size = new Size(662, 73);
        lblInstructions.TabIndex = 0;
        lblInstructions.Text = "1. Retire el cable.\r\n2. Revise la posicion indicada.\r\n3. Inserte nuevamente el cable en el pin correcto.";
        // 
        // pnlStatus
        // 
        pnlStatus.BackColor = Color.MistyRose;
        pnlStatus.BorderStyle = BorderStyle.FixedSingle;
        pnlStatus.Controls.Add(lblStatusCaption);
        pnlStatus.Controls.Add(lblStatus);
        pnlStatus.Location = new Point(27, 286);
        pnlStatus.Name = "pnlStatus";
        pnlStatus.Size = new Size(705, 64);
        pnlStatus.TabIndex = 5;
        // 
        // lblStatusCaption
        // 
        lblStatusCaption.AutoSize = true;
        lblStatusCaption.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblStatusCaption.Location = new Point(17, 9);
        lblStatusCaption.Name = "lblStatusCaption";
        lblStatusCaption.Size = new Size(62, 19);
        lblStatusCaption.TabIndex = 0;
        lblStatusCaption.Text = "ESTADO";
        // 
        // lblStatus
        // 
        lblStatus.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        lblStatus.ForeColor = Color.Firebrick;
        lblStatus.Location = new Point(17, 29);
        lblStatus.Name = "lblStatus";
        lblStatus.Size = new Size(663, 27);
        lblStatus.TabIndex = 1;
        lblStatus.Text = "Error presente - esperando correccion fisica";
        // 
        // btnAuthorize
        // 
        btnAuthorize.BackColor = Color.DarkOrange;
        btnAuthorize.FlatStyle = FlatStyle.Flat;
        btnAuthorize.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        btnAuthorize.ForeColor = Color.White;
        btnAuthorize.Location = new Point(562, 395);
        btnAuthorize.Name = "btnAuthorize";
        btnAuthorize.Size = new Size(170, 43);
        btnAuthorize.TabIndex = 6;
        btnAuthorize.Text = "Desbloquear";
        btnAuthorize.UseVisualStyleBackColor = false;
        btnAuthorize.Click += btnAuthorize_Click;
        // 
        // btnAcknowledge
        // 
        btnAcknowledge.BackColor = Color.SeaGreen;
        btnAcknowledge.Enabled = false;
        btnAcknowledge.FlatStyle = FlatStyle.Flat;
        btnAcknowledge.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        btnAcknowledge.ForeColor = Color.White;
        btnAcknowledge.Location = new Point(473, 578);
        btnAcknowledge.Name = "btnAcknowledge";
        btnAcknowledge.Size = new Size(152, 43);
        btnAcknowledge.TabIndex = 7;
        btnAcknowledge.Text = "Continuar";
        btnAcknowledge.UseVisualStyleBackColor = false;
        btnAcknowledge.Visible = false;
        // 
        // btnClose
        // 
        btnClose.DialogResult = DialogResult.Cancel;
        btnClose.FlatStyle = FlatStyle.Flat;
        btnClose.Font = new Font("Segoe UI", 10F);
        btnClose.Location = new Point(631, 578);
        btnClose.Name = "btnClose";
        btnClose.Size = new Size(101, 43);
        btnClose.TabIndex = 8;
        btnClose.Text = "Cerrar";
        btnClose.UseVisualStyleBackColor = true;
        btnClose.Visible = false;
        // 
        // lblAuthorization
        // 
        lblAuthorization.AutoSize = true;
        lblAuthorization.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblAuthorization.Location = new Point(27, 368);
        lblAuthorization.Name = "lblAuthorization";
        lblAuthorization.Size = new Size(246, 19);
        lblAuthorization.TabIndex = 9;
        lblAuthorization.Text = "Código de autorización de Calidad:";
        // 
        // txtAuthorization
        // 
        txtAuthorization.Font = new Font("Segoe UI", 14F);
        txtAuthorization.Location = new Point(27, 395);
        txtAuthorization.Name = "txtAuthorization";
        txtAuthorization.Size = new Size(517, 32);
        txtAuthorization.TabIndex = 10;
        txtAuthorization.UseSystemPasswordChar = true;
        txtAuthorization.KeyDown += txtAuthorization_KeyDown;
        // 
        // StationErrorAlertForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.WhiteSmoke;
        CancelButton = btnClose;
        ClientSize = new Size(760, 461);
        Controls.Add(txtAuthorization);
        Controls.Add(lblAuthorization);
        Controls.Add(btnClose);
        Controls.Add(btnAcknowledge);
        Controls.Add(btnAuthorize);
        Controls.Add(pnlStatus);
        Controls.Add(lblErrorType);
        Controls.Add(lblErrorTypeCaption);
        Controls.Add(pnlHeader);
        FormBorderStyle = FormBorderStyle.None;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "StationErrorAlertForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Alerta de estacion";
        TopMost = true;
        Activated += StationErrorAlertForm_Activated;
        Shown += StationErrorAlertForm_Shown;
        pnlHeader.ResumeLayout(false);
        grpDetection.ResumeLayout(false);
        grpDetection.PerformLayout();
        grpInstructions.ResumeLayout(false);
        pnlStatus.ResumeLayout(false);
        pnlStatus.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }
}
