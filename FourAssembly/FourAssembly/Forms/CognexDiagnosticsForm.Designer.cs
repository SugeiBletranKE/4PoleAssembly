namespace FourAssembly.Forms;

partial class CognexDiagnosticsForm
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
        lstCognexData = new ListBox();
        btnClear = new Button();
        lblTotalScans = new Label();
        SuspendLayout();

        // lblTitle
        lblTitle.AutoSize = true;
        lblTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
        lblTitle.Location = new Point(12, 9);
        lblTitle.Name = "lblTitle";
        lblTitle.Size = new Size(200, 32);
        lblTitle.TabIndex = 0;
        lblTitle.Text = "Cognex Real-time Feed";

        // lstCognexData
        lstCognexData.Font = new Font("Consolas", 10F);
        lstCognexData.FormattingEnabled = true;
        lstCognexData.ItemHeight = 17;
        lstCognexData.Location = new Point(12, 45);
        lstCognexData.Name = "lstCognexData";
        lstCognexData.Size = new Size(560, 310);
        lstCognexData.TabIndex = 1;

        // lblTotalScans
        lblTotalScans.AutoSize = true;
        lblTotalScans.Font = new Font("Segoe UI", 11F);
        lblTotalScans.Location = new Point(12, 365);
        lblTotalScans.Name = "lblTotalScans";
        lblTotalScans.Size = new Size(100, 20);
        lblTotalScans.TabIndex = 2;
        lblTotalScans.Text = "Total Scans: 0";

        // btnClear
        btnClear.Font = new Font("Segoe UI", 10F);
        btnClear.Location = new Point(470, 365);
        btnClear.Name = "btnClear";
        btnClear.Size = new Size(102, 30);
        btnClear.TabIndex = 3;
        btnClear.Text = "Clear Log";
        btnClear.UseVisualStyleBackColor = true;
        btnClear.Click += BtnClear_Click;

        // CognexDiagnosticsForm
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(584, 407);
        Controls.Add(btnClear);
        Controls.Add(lblTotalScans);
        Controls.Add(lstCognexData);
        Controls.Add(lblTitle);
        Name = "CognexDiagnosticsForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Cognex Diagnostics";
        ResumeLayout(false);
        PerformLayout();
    }

    private Label lblTitle = null!;
    private ListBox lstCognexData = null!;
    private Button btnClear = null!;
    private Label lblTotalScans = null!;
}
