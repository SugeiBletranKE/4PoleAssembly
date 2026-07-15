namespace FourAssembly.Forms;

partial class PlcDiagnosticsForm
{
    private System.ComponentModel.IContainer components = null;

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
        lblTitle = new Label();
        lblEndpoint = new Label();
        lblStatusCaption = new Label();
        lblStatus = new Label();
        btnTestConnection = new Button();
        btnDisconnect = new Button();
        grpReadCoil = new GroupBox();
        btnReadCoil = new Button();
        txtCoil = new TextBox();
        lblCoil = new Label();
        grpSendPulse = new GroupBox();
        txtPulseMs = new TextBox();
        lblPulseMs = new Label();
        btnSendPulse = new Button();
        txtPulseCoil = new TextBox();
        lblPulseCoil = new Label();
        grpWriteAnyRegister = new GroupBox();
        cmbAnyType = new ComboBox();
        lblAnyType = new Label();
        txtAnyValue = new TextBox();
        lblAnyValue = new Label();
        btnWriteAnyRegister = new Button();
        txtAnyRegister = new TextBox();
        lblAnyRegister = new Label();
        grpWriteRegister = new GroupBox();
        txtRegisterValue = new TextBox();
        lblRegisterValue = new Label();
        btnWriteRegister = new Button();
        txtRegister = new TextBox();
        lblRegister = new Label();
        grpWriteStringRegister = new GroupBox();
        txtStringValue = new TextBox();
        lblStringValue = new Label();
        btnWriteStringRegister = new Button();
        txtStringRegister = new TextBox();
        lblStringRegister = new Label();
        lstLog = new ListBox();
        grpReadCoil.SuspendLayout();
        grpSendPulse.SuspendLayout();
        grpWriteAnyRegister.SuspendLayout();
        grpWriteRegister.SuspendLayout();
        grpWriteStringRegister.SuspendLayout();
        SuspendLayout();
        // 
        // lblTitle
        // 
        lblTitle.AutoSize = true;
        lblTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
        lblTitle.Location = new Point(12, 9);
        lblTitle.Name = "lblTitle";
        lblTitle.Size = new Size(153, 25);
        lblTitle.TabIndex = 0;
        lblTitle.Text = "PLC Diagnostics";
        // 
        // lblEndpoint
        // 
        lblEndpoint.AutoSize = true;
        lblEndpoint.Font = new Font("Segoe UI", 10F);
        lblEndpoint.Location = new Point(14, 43);
        lblEndpoint.Name = "lblEndpoint";
        lblEndpoint.Size = new Size(33, 19);
        lblEndpoint.TabIndex = 1;
        lblEndpoint.Text = "PLC";
        // 
        // lblStatusCaption
        // 
        lblStatusCaption.AutoSize = true;
        lblStatusCaption.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblStatusCaption.Location = new Point(14, 75);
        lblStatusCaption.Name = "lblStatusCaption";
        lblStatusCaption.Size = new Size(53, 19);
        lblStatusCaption.TabIndex = 2;
        lblStatusCaption.Text = "Status:";
        // 
        // lblStatus
        // 
        lblStatus.AutoSize = true;
        lblStatus.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblStatus.Location = new Point(72, 75);
        lblStatus.Name = "lblStatus";
        lblStatus.Size = new Size(51, 19);
        lblStatus.TabIndex = 3;
        lblStatus.Text = "Ready";
        // 
        // btnTestConnection
        // 
        btnTestConnection.Font = new Font("Segoe UI", 10F);
        btnTestConnection.Location = new Point(14, 110);
        btnTestConnection.Name = "btnTestConnection";
        btnTestConnection.Size = new Size(160, 34);
        btnTestConnection.TabIndex = 4;
        btnTestConnection.Text = "Test Connection";
        btnTestConnection.UseVisualStyleBackColor = true;
        btnTestConnection.Click += BtnTestConnection_Click;
        // 
        // btnDisconnect
        // 
        btnDisconnect.Font = new Font("Segoe UI", 10F);
        btnDisconnect.Location = new Point(184, 110);
        btnDisconnect.Name = "btnDisconnect";
        btnDisconnect.Size = new Size(120, 34);
        btnDisconnect.TabIndex = 5;
        btnDisconnect.Text = "Disconnect";
        btnDisconnect.UseVisualStyleBackColor = true;
        btnDisconnect.Click += BtnDisconnect_Click;
        // 
        // grpReadCoil
        // 
        grpReadCoil.Controls.Add(btnReadCoil);
        grpReadCoil.Controls.Add(txtCoil);
        grpReadCoil.Controls.Add(lblCoil);
        grpReadCoil.Font = new Font("Segoe UI", 10F);
        grpReadCoil.Location = new Point(14, 160);
        grpReadCoil.Name = "grpReadCoil";
        grpReadCoil.Size = new Size(370, 75);
        grpReadCoil.TabIndex = 6;
        grpReadCoil.TabStop = false;
        grpReadCoil.Text = "Read Coil";
        // 
        // btnReadCoil
        // 
        btnReadCoil.Location = new Point(228, 28);
        btnReadCoil.Name = "btnReadCoil";
        btnReadCoil.Size = new Size(120, 30);
        btnReadCoil.TabIndex = 2;
        btnReadCoil.Text = "Read";
        btnReadCoil.UseVisualStyleBackColor = true;
        btnReadCoil.Click += BtnReadCoil_Click;
        // 
        // txtCoil
        // 
        txtCoil.Location = new Point(102, 31);
        txtCoil.Name = "txtCoil";
        txtCoil.Size = new Size(100, 25);
        txtCoil.TabIndex = 1;
        txtCoil.Text = "101";
        // 
        // lblCoil
        // 
        lblCoil.AutoSize = true;
        lblCoil.Location = new Point(15, 34);
        lblCoil.Name = "lblCoil";
        lblCoil.Size = new Size(83, 19);
        lblCoil.TabIndex = 0;
        lblCoil.Text = "Coil address";
        // 
        // grpSendPulse
        // 
        grpSendPulse.Controls.Add(txtPulseMs);
        grpSendPulse.Controls.Add(lblPulseMs);
        grpSendPulse.Controls.Add(btnSendPulse);
        grpSendPulse.Controls.Add(txtPulseCoil);
        grpSendPulse.Controls.Add(lblPulseCoil);
        grpSendPulse.Font = new Font("Segoe UI", 10F);
        grpSendPulse.Location = new Point(14, 245);
        grpSendPulse.Name = "grpSendPulse";
        grpSendPulse.Size = new Size(520, 75);
        grpSendPulse.TabIndex = 7;
        grpSendPulse.TabStop = false;
        grpSendPulse.Text = "Send Coil Pulse";
        // 
        // txtPulseMs
        // 
        txtPulseMs.Location = new Point(288, 31);
        txtPulseMs.Name = "txtPulseMs";
        txtPulseMs.Size = new Size(70, 25);
        txtPulseMs.TabIndex = 3;
        txtPulseMs.Text = "500";
        // 
        // lblPulseMs
        // 
        lblPulseMs.AutoSize = true;
        lblPulseMs.Location = new Point(220, 34);
        lblPulseMs.Name = "lblPulseMs";
        lblPulseMs.Size = new Size(60, 19);
        lblPulseMs.TabIndex = 2;
        lblPulseMs.Text = "Time ms";
        // 
        // btnSendPulse
        // 
        btnSendPulse.Location = new Point(384, 28);
        btnSendPulse.Name = "btnSendPulse";
        btnSendPulse.Size = new Size(120, 30);
        btnSendPulse.TabIndex = 4;
        btnSendPulse.Text = "Send Pulse";
        btnSendPulse.UseVisualStyleBackColor = true;
        btnSendPulse.Click += BtnSendPulse_Click;
        // 
        // txtPulseCoil
        // 
        txtPulseCoil.Location = new Point(102, 31);
        txtPulseCoil.Name = "txtPulseCoil";
        txtPulseCoil.Size = new Size(100, 25);
        txtPulseCoil.TabIndex = 1;
        txtPulseCoil.Text = "1";
        // 
        // lblPulseCoil
        // 
        lblPulseCoil.AutoSize = true;
        lblPulseCoil.Location = new Point(15, 34);
        lblPulseCoil.Name = "lblPulseCoil";
        lblPulseCoil.Size = new Size(83, 19);
        lblPulseCoil.TabIndex = 0;
        lblPulseCoil.Text = "Coil address";
        // 
        // grpWriteAnyRegister
        // 
        grpWriteAnyRegister.Controls.Add(cmbAnyType);
        grpWriteAnyRegister.Controls.Add(lblAnyType);
        grpWriteAnyRegister.Controls.Add(txtAnyValue);
        grpWriteAnyRegister.Controls.Add(lblAnyValue);
        grpWriteAnyRegister.Controls.Add(btnWriteAnyRegister);
        grpWriteAnyRegister.Controls.Add(txtAnyRegister);
        grpWriteAnyRegister.Controls.Add(lblAnyRegister);
        grpWriteAnyRegister.Font = new Font("Segoe UI", 10F);
        grpWriteAnyRegister.Location = new Point(14, 330);
        grpWriteAnyRegister.Name = "grpWriteAnyRegister";
        grpWriteAnyRegister.Size = new Size(520, 75);
        grpWriteAnyRegister.TabIndex = 8;
        grpWriteAnyRegister.TabStop = false;
        grpWriteAnyRegister.Text = "Write Memory / Register";
        // 
        // cmbAnyType
        // 
        cmbAnyType.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbAnyType.FormattingEnabled = true;
        cmbAnyType.Items.AddRange(new object[] { "Number", "String" });
        cmbAnyType.Location = new Point(328, 31);
        cmbAnyType.Name = "cmbAnyType";
        cmbAnyType.Size = new Size(86, 25);
        cmbAnyType.TabIndex = 5;
        // 
        // lblAnyType
        // 
        lblAnyType.AutoSize = true;
        lblAnyType.Location = new Point(286, 34);
        lblAnyType.Name = "lblAnyType";
        lblAnyType.Size = new Size(36, 19);
        lblAnyType.TabIndex = 4;
        lblAnyType.Text = "Type";
        // 
        // txtAnyValue
        // 
        txtAnyValue.Location = new Point(208, 31);
        txtAnyValue.Name = "txtAnyValue";
        txtAnyValue.Size = new Size(70, 25);
        txtAnyValue.TabIndex = 3;
        txtAnyValue.Text = "1";
        // 
        // lblAnyValue
        // 
        lblAnyValue.AutoSize = true;
        lblAnyValue.Location = new Point(160, 34);
        lblAnyValue.Name = "lblAnyValue";
        lblAnyValue.Size = new Size(42, 19);
        lblAnyValue.TabIndex = 2;
        lblAnyValue.Text = "Value";
        // 
        // btnWriteAnyRegister
        // 
        btnWriteAnyRegister.Location = new Point(426, 28);
        btnWriteAnyRegister.Name = "btnWriteAnyRegister";
        btnWriteAnyRegister.Size = new Size(78, 30);
        btnWriteAnyRegister.TabIndex = 6;
        btnWriteAnyRegister.Text = "Write";
        btnWriteAnyRegister.UseVisualStyleBackColor = true;
        btnWriteAnyRegister.Click += BtnWriteAnyRegister_Click;
        // 
        // txtAnyRegister
        // 
        txtAnyRegister.Location = new Point(82, 31);
        txtAnyRegister.Name = "txtAnyRegister";
        txtAnyRegister.Size = new Size(70, 25);
        txtAnyRegister.TabIndex = 1;
        txtAnyRegister.Text = "2";
        // 
        // lblAnyRegister
        // 
        lblAnyRegister.AutoSize = true;
        lblAnyRegister.Location = new Point(15, 34);
        lblAnyRegister.Name = "lblAnyRegister";
        lblAnyRegister.Size = new Size(61, 19);
        lblAnyRegister.TabIndex = 0;
        lblAnyRegister.Text = "Register";
        // 
        // grpWriteRegister
        // 
        grpWriteRegister.Controls.Add(txtRegisterValue);
        grpWriteRegister.Controls.Add(lblRegisterValue);
        grpWriteRegister.Controls.Add(btnWriteRegister);
        grpWriteRegister.Controls.Add(txtRegister);
        grpWriteRegister.Controls.Add(lblRegister);
        grpWriteRegister.Font = new Font("Segoe UI", 10F);
        grpWriteRegister.Location = new Point(14, 330);
        grpWriteRegister.Name = "grpWriteRegister";
        grpWriteRegister.Size = new Size(520, 75);
        grpWriteRegister.TabIndex = 8;
        grpWriteRegister.TabStop = false;
        grpWriteRegister.Text = "Write Holding Register";
        grpWriteRegister.Visible = false;
        // 
        // txtRegisterValue
        // 
        txtRegisterValue.Location = new Point(288, 31);
        txtRegisterValue.Name = "txtRegisterValue";
        txtRegisterValue.Size = new Size(70, 25);
        txtRegisterValue.TabIndex = 3;
        txtRegisterValue.Text = "1";
        // 
        // lblRegisterValue
        // 
        lblRegisterValue.AutoSize = true;
        lblRegisterValue.Location = new Point(240, 34);
        lblRegisterValue.Name = "lblRegisterValue";
        lblRegisterValue.Size = new Size(42, 19);
        lblRegisterValue.TabIndex = 2;
        lblRegisterValue.Text = "Value";
        // 
        // btnWriteRegister
        // 
        btnWriteRegister.Location = new Point(384, 28);
        btnWriteRegister.Name = "btnWriteRegister";
        btnWriteRegister.Size = new Size(120, 30);
        btnWriteRegister.TabIndex = 4;
        btnWriteRegister.Text = "Write";
        btnWriteRegister.UseVisualStyleBackColor = true;
        btnWriteRegister.Click += BtnWriteRegister_Click;
        // 
        // txtRegister
        // 
        txtRegister.Location = new Point(118, 31);
        txtRegister.Name = "txtRegister";
        txtRegister.Size = new Size(100, 25);
        txtRegister.TabIndex = 1;
        txtRegister.Text = "10";
        // 
        // lblRegister
        // 
        lblRegister.AutoSize = true;
        lblRegister.Location = new Point(15, 34);
        lblRegister.Name = "lblRegister";
        lblRegister.Size = new Size(93, 19);
        lblRegister.TabIndex = 0;
        lblRegister.Text = "Register addr.";
        // 
        // grpWriteStringRegister
        // 
        grpWriteStringRegister.Controls.Add(txtStringValue);
        grpWriteStringRegister.Controls.Add(lblStringValue);
        grpWriteStringRegister.Controls.Add(btnWriteStringRegister);
        grpWriteStringRegister.Controls.Add(txtStringRegister);
        grpWriteStringRegister.Controls.Add(lblStringRegister);
        grpWriteStringRegister.Font = new Font("Segoe UI", 10F);
        grpWriteStringRegister.Location = new Point(14, 415);
        grpWriteStringRegister.Name = "grpWriteStringRegister";
        grpWriteStringRegister.Size = new Size(520, 75);
        grpWriteStringRegister.TabIndex = 9;
        grpWriteStringRegister.TabStop = false;
        grpWriteStringRegister.Text = "Write String Register";
        grpWriteStringRegister.Visible = false;
        // 
        // txtStringValue
        // 
        txtStringValue.Location = new Point(288, 31);
        txtStringValue.MaxLength = 2;
        txtStringValue.Name = "txtStringValue";
        txtStringValue.Size = new Size(70, 25);
        txtStringValue.TabIndex = 3;
        txtStringValue.Text = "A";
        // 
        // lblStringValue
        // 
        lblStringValue.AutoSize = true;
        lblStringValue.Location = new Point(240, 34);
        lblStringValue.Name = "lblStringValue";
        lblStringValue.Size = new Size(42, 19);
        lblStringValue.TabIndex = 2;
        lblStringValue.Text = "Value";
        // 
        // btnWriteStringRegister
        // 
        btnWriteStringRegister.Location = new Point(384, 28);
        btnWriteStringRegister.Name = "btnWriteStringRegister";
        btnWriteStringRegister.Size = new Size(120, 30);
        btnWriteStringRegister.TabIndex = 4;
        btnWriteStringRegister.Text = "Write String";
        btnWriteStringRegister.UseVisualStyleBackColor = true;
        btnWriteStringRegister.Click += BtnWriteStringRegister_Click;
        // 
        // txtStringRegister
        // 
        txtStringRegister.Location = new Point(118, 31);
        txtStringRegister.Name = "txtStringRegister";
        txtStringRegister.Size = new Size(100, 25);
        txtStringRegister.TabIndex = 1;
        txtStringRegister.Text = "0";
        // 
        // lblStringRegister
        // 
        lblStringRegister.AutoSize = true;
        lblStringRegister.Location = new Point(15, 34);
        lblStringRegister.Name = "lblStringRegister";
        lblStringRegister.Size = new Size(93, 19);
        lblStringRegister.TabIndex = 0;
        lblStringRegister.Text = "Register addr.";
        // 
        // lstLog
        // 
        lstLog.Font = new Font("Consolas", 9F);
        lstLog.FormattingEnabled = true;
        lstLog.ItemHeight = 14;
        lstLog.Location = new Point(14, 420);
        lstLog.Name = "lstLog";
        lstLog.Size = new Size(520, 144);
        lstLog.TabIndex = 10;
        // 
        // PlcDiagnosticsForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(548, 581);
        Controls.Add(lstLog);
        Controls.Add(grpWriteStringRegister);
        Controls.Add(grpWriteRegister);
        Controls.Add(grpWriteAnyRegister);
        Controls.Add(grpSendPulse);
        Controls.Add(grpReadCoil);
        Controls.Add(btnDisconnect);
        Controls.Add(btnTestConnection);
        Controls.Add(lblStatus);
        Controls.Add(lblStatusCaption);
        Controls.Add(lblEndpoint);
        Controls.Add(lblTitle);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "PlcDiagnosticsForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "PLC Diagnostics";
        Load += PlcDiagnosticsForm_Load_1;
        cmbAnyType.SelectedIndex = 0;
        grpReadCoil.ResumeLayout(false);
        grpReadCoil.PerformLayout();
        grpSendPulse.ResumeLayout(false);
        grpSendPulse.PerformLayout();
        grpWriteAnyRegister.ResumeLayout(false);
        grpWriteAnyRegister.PerformLayout();
        grpWriteRegister.ResumeLayout(false);
        grpWriteRegister.PerformLayout();
        grpWriteStringRegister.ResumeLayout(false);
        grpWriteStringRegister.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    private Label lblTitle = null!;
    private Label lblEndpoint = null!;
    private Label lblStatusCaption = null!;
    private Label lblStatus = null!;
    private Button btnTestConnection = null!;
    private Button btnDisconnect = null!;
    private GroupBox grpReadCoil = null!;
    private Button btnReadCoil = null!;
    private TextBox txtCoil = null!;
    private Label lblCoil = null!;
    private GroupBox grpSendPulse = null!;
    private TextBox txtPulseMs = null!;
    private Label lblPulseMs = null!;
    private Button btnSendPulse = null!;
    private TextBox txtPulseCoil = null!;
    private Label lblPulseCoil = null!;
    private GroupBox grpWriteAnyRegister = null!;
    private ComboBox cmbAnyType = null!;
    private Label lblAnyType = null!;
    private TextBox txtAnyValue = null!;
    private Label lblAnyValue = null!;
    private Button btnWriteAnyRegister = null!;
    private TextBox txtAnyRegister = null!;
    private Label lblAnyRegister = null!;
    private GroupBox grpWriteRegister = null!;
    private TextBox txtRegisterValue = null!;
    private Label lblRegisterValue = null!;
    private Button btnWriteRegister = null!;
    private TextBox txtRegister = null!;
    private Label lblRegister = null!;
    private GroupBox grpWriteStringRegister = null!;
    private TextBox txtStringValue = null!;
    private Label lblStringValue = null!;
    private Button btnWriteStringRegister = null!;
    private TextBox txtStringRegister = null!;
    private Label lblStringRegister = null!;
    private ListBox lstLog = null!;
}
