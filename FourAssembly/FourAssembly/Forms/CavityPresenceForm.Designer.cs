namespace FourAssembly.Forms;

partial class CavityPresenceForm
{
    private System.ComponentModel.IContainer components = null!;
    private Panel pnlHeader = null!;
    private Label lblTitle = null!;
    private Label lblEndpoint = null!;
    private Button btnRefresh = null!;
    private FlowLayoutPanel flowStations = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _refreshTimer.Stop();
            components?.Dispose();
        }

        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        pnlHeader = new Panel();
        lblEndpoint = new Label();
        lblTitle = new Label();
        btnRefresh = new Button();
        flowStations = new FlowLayoutPanel();
        pnlHeader.SuspendLayout();
        SuspendLayout();
        // 
        // pnlHeader
        // 
        pnlHeader.BackColor = Color.FromArgb(20, 55, 100);
        pnlHeader.Controls.Add(lblEndpoint);
        pnlHeader.Controls.Add(lblTitle);
        pnlHeader.Controls.Add(btnRefresh);
        pnlHeader.Dock = DockStyle.Top;
        pnlHeader.Location = new Point(0, 0);
        pnlHeader.Name = "pnlHeader";
        pnlHeader.Padding = new Padding(20, 12, 20, 8);
        pnlHeader.Size = new Size(1234, 82);
        pnlHeader.TabIndex = 0;
        // 
        // lblEndpoint
        // 
        lblEndpoint.AutoSize = true;
        lblEndpoint.Font = new Font("Segoe UI", 10F);
        lblEndpoint.ForeColor = Color.LightSteelBlue;
        lblEndpoint.Location = new Point(21, 48);
        lblEndpoint.Name = "lblEndpoint";
        lblEndpoint.Size = new Size(287, 19);
        lblEndpoint.TabIndex = 1;
        lblEndpoint.Text = "PLC --  •  Actualización 250 ms  •  Solo lectura";
        // 
        // lblTitle
        // 
        lblTitle.AutoSize = true;
        lblTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
        lblTitle.ForeColor = Color.White;
        lblTitle.Location = new Point(18, 8);
        lblTitle.Name = "lblTitle";
        lblTitle.Size = new Size(292, 32);
        lblTitle.TabIndex = 0;
        lblTitle.Text = "VISTA DE PRESENCIA";
        // 
        // btnRefresh
        // 
        btnRefresh.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnRefresh.BackColor = Color.White;
        btnRefresh.FlatStyle = FlatStyle.Flat;
        btnRefresh.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        btnRefresh.ForeColor = Color.FromArgb(20, 55, 100);
        btnRefresh.Location = new Point(1045, 20);
        btnRefresh.Name = "btnRefresh";
        btnRefresh.Size = new Size(165, 42);
        btnRefresh.TabIndex = 2;
        btnRefresh.Text = "↻ Refrescar";
        btnRefresh.UseVisualStyleBackColor = false;
        btnRefresh.Click += BtnRefresh_Click;
        // 
        // flowStations
        // 
        flowStations.AutoScroll = true;
        flowStations.Dock = DockStyle.Fill;
        flowStations.FlowDirection = FlowDirection.TopDown;
        flowStations.Location = new Point(0, 82);
        flowStations.Name = "flowStations";
        flowStations.Padding = new Padding(16);
        flowStations.Size = new Size(1234, 639);
        flowStations.TabIndex = 1;
        flowStations.WrapContents = false;
        // 
        // CavityPresenceForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(238, 242, 247);
        ClientSize = new Size(1234, 721);
        Controls.Add(flowStations);
        Controls.Add(pnlHeader);
        MinimumSize = new Size(1050, 620);
        Name = "CavityPresenceForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Vista de presencia PLC";
        pnlHeader.ResumeLayout(false);
        pnlHeader.PerformLayout();
        ResumeLayout(false);
    }
}
