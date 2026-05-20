namespace FourAssembly.Controls;

partial class StationControl
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
        lblStationTitle = new Label();
        lblInstruction = new Label();
        txtScanInput = new TextBox();
        lstCables = new ListView();
        lblCableCount = new Label();
        btnFinalizarCables = new Button();
        panelStatus = new Panel();
        lblStatus = new Label();
        btnReset = new Button();
        panelStatus.SuspendLayout();
        SuspendLayout();
        // 
        // lblStationTitle
        // 
        lblStationTitle.AutoSize = true;
        lblStationTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
        lblStationTitle.Location = new Point(10, 9);
        lblStationTitle.Name = "lblStationTitle";
        lblStationTitle.Size = new Size(105, 30);
        lblStationTitle.TabIndex = 0;
        lblStationTitle.Text = "Station 1";
        // 
        // lblInstruction
        // 
        lblInstruction.Font = new Font("Segoe UI", 14F);
        lblInstruction.ForeColor = Color.Black;
        lblInstruction.Location = new Point(10, 45);
        lblInstruction.Name = "lblInstruction";
        lblInstruction.Size = new Size(700, 75);
        lblInstruction.TabIndex = 1;
        lblInstruction.Text = "Scan the STATION barcode on this PC";
        // 
        // txtScanInput
        // 
        txtScanInput.Font = new Font("Segoe UI", 12F);
        txtScanInput.Location = new Point(10, 128);
        txtScanInput.Margin = new Padding(3, 2, 3, 2);
        txtScanInput.Name = "txtScanInput";
        txtScanInput.Size = new Size(350, 29);
        txtScanInput.TabIndex = 2;
        txtScanInput.TextChanged += txtScanInput_TextChanged;
        txtScanInput.KeyDown += TxtScanInput_KeyDown;
        // 
        // lstCables
        // 
        lstCables.Font = new Font("Segoe UI", 11F);
        lstCables.Location = new Point(10, 202);
        lstCables.Margin = new Padding(3, 2, 3, 2);
        lstCables.Name = "lstCables";
        lstCables.Size = new Size(350, 54);
        lstCables.TabIndex = 3;
        lstCables.UseCompatibleStateImageBehavior = false;
        lstCables.View = View.List;
        lstCables.Visible = false;
        // 
        // lblCableCount
        // 
        lblCableCount.AutoSize = true;
        lblCableCount.Font = new Font("Segoe UI", 11F);
        lblCableCount.Location = new Point(10, 262);
        lblCableCount.Name = "lblCableCount";
        lblCableCount.Size = new Size(68, 20);
        lblCableCount.TabIndex = 4;
        lblCableCount.Text = "Cables: 0";
        lblCableCount.Visible = false;
        // 
        // btnFinalizarCables
        // 
        btnFinalizarCables.Font = new Font("Segoe UI", 11F);
        btnFinalizarCables.Location = new Point(10, 281);
        btnFinalizarCables.Margin = new Padding(3, 2, 3, 2);
        btnFinalizarCables.Name = "btnFinalizarCables";
        btnFinalizarCables.Size = new Size(131, 30);
        btnFinalizarCables.TabIndex = 5;
        btnFinalizarCables.Text = "Finalizar escaneo";
        btnFinalizarCables.UseVisualStyleBackColor = true;
        btnFinalizarCables.Visible = false;
        btnFinalizarCables.Click += BtnFinalizarCables_Click;
        // 
        // panelStatus
        // 
        panelStatus.BackColor = Color.LightGray;
        panelStatus.BorderStyle = BorderStyle.FixedSingle;
        panelStatus.Controls.Add(lblStatus);
        panelStatus.Dock = DockStyle.Bottom;
        panelStatus.Location = new Point(0, 510);
        panelStatus.Margin = new Padding(3, 2, 3, 2);
        panelStatus.Name = "panelStatus";
        panelStatus.Size = new Size(875, 60);
        panelStatus.TabIndex = 3;
        // 
        // lblStatus
        // 
        lblStatus.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        lblStatus.ForeColor = Color.Black;
        lblStatus.Location = new Point(10, 9);
        lblStatus.Name = "lblStatus";
        lblStatus.Size = new Size(858, 38);
        lblStatus.TabIndex = 0;
        lblStatus.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // btnReset
        // 
        btnReset.Font = new Font("Segoe UI", 11F);
        btnReset.Location = new Point(10, 165);
        btnReset.Margin = new Padding(3, 2, 3, 2);
        btnReset.Name = "btnReset";
        btnReset.Size = new Size(88, 30);
        btnReset.TabIndex = 6;
        btnReset.Text = "Reset";
        btnReset.UseVisualStyleBackColor = true;
        btnReset.Click += BtnReset_Click;
        // 
        // StationControl
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(btnFinalizarCables);
        Controls.Add(lblCableCount);
        Controls.Add(lstCables);
        Controls.Add(btnReset);
        Controls.Add(txtScanInput);
        Controls.Add(lblInstruction);
        Controls.Add(lblStationTitle);
        Controls.Add(panelStatus);
        Margin = new Padding(3, 2, 3, 2);
        Name = "StationControl";
        Size = new Size(875, 570);
        panelStatus.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }

    private Label lblStationTitle = null!;
    private Label lblInstruction = null!;
    private TextBox txtScanInput = null!;
    private ListView lstCables = null!;
    private Label lblCableCount = null!;
    private Button btnFinalizarCables = null!;
    private Panel panelStatus = null!;
    private Label lblStatus = null!;
    private Button btnReset = null!;
}
