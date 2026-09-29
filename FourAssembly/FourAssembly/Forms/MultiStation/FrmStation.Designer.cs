namespace FourAssembly.MultiStation
{
    partial class FrmStation
    {
        private System.ComponentModel.IContainer components = null;
        private Panel statusPanel;
        private Label _lblPartNumber;
        private Label _lblStatus;
        private Label _lblProcessState;
        private Panel stationPanelHost;
        private Label lblDesignerHint;

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
            statusPanel = new Panel();
            _lblProcessState = new Label();
            _lblStatus = new Label();
            _lblPartNumber = new Label();
            stationPanelHost = new Panel();
            lblDesignerHint = new Label();
            statusPanel.SuspendLayout();
            stationPanelHost.SuspendLayout();
            SuspendLayout();
            // 
            // statusPanel
            // 
            statusPanel.BackColor = Color.DarkGray;
            statusPanel.Controls.Add(_lblProcessState);
            statusPanel.Controls.Add(_lblStatus);
            statusPanel.Controls.Add(_lblPartNumber);
            statusPanel.Dock = DockStyle.Top;
            statusPanel.Location = new Point(0, 0);
            statusPanel.Name = "statusPanel";
            statusPanel.Size = new Size(900, 80);
            statusPanel.TabIndex = 0;
            // 
            // _lblProcessState
            // 
            _lblProcessState.AutoSize = true;
            _lblProcessState.Font = new Font("Segoe UI", 10F);
            _lblProcessState.ForeColor = Color.Orange;
            _lblProcessState.Location = new Point(350, 15);
            _lblProcessState.Name = "_lblProcessState";
            _lblProcessState.Size = new Size(134, 19);
            _lblProcessState.TabIndex = 2;
            _lblProcessState.Text = "Esperando proceso...";
            // 
            // _lblStatus
            // 
            _lblStatus.AutoSize = true;
            _lblStatus.Font = new Font("Segoe UI", 11F);
            _lblStatus.ForeColor = Color.White;
            _lblStatus.Location = new Point(10, 50);
            _lblStatus.Name = "_lblStatus";
            _lblStatus.Size = new Size(124, 20);
            _lblStatus.TabIndex = 1;
            _lblStatus.Text = "Status: DISABLED";
            // 
            // _lblPartNumber
            // 
            _lblPartNumber.AutoSize = true;
            _lblPartNumber.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            _lblPartNumber.ForeColor = Color.White;
            _lblPartNumber.Location = new Point(10, 7);
            _lblPartNumber.Name = "_lblPartNumber";
            _lblPartNumber.Size = new Size(213, 37);
            _lblPartNumber.TabIndex = 0;
            _lblPartNumber.Text = "PartNumber: --";
            // 
            // stationPanelHost
            // 
            stationPanelHost.BackColor = Color.WhiteSmoke;
            stationPanelHost.Controls.Add(lblDesignerHint);
            stationPanelHost.Dock = DockStyle.Fill;
            stationPanelHost.Location = new Point(0, 80);
            stationPanelHost.Name = "stationPanelHost";
            stationPanelHost.Size = new Size(900, 520);
            stationPanelHost.TabIndex = 1;
            // 
            // lblDesignerHint
            // 
            lblDesignerHint.Dock = DockStyle.Fill;
            lblDesignerHint.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblDesignerHint.ForeColor = Color.DimGray;
            lblDesignerHint.Location = new Point(0, 0);
            lblDesignerHint.Name = "lblDesignerHint";
            lblDesignerHint.Size = new Size(900, 520);
            lblDesignerHint.TabIndex = 0;
            lblDesignerHint.Text = "STATION PANEL\r\n\r\nAbra StationPanel.cs [Diseño] para editar sus componentes";
            lblDesignerHint.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // FrmStation
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(900, 600);
            Controls.Add(stationPanelHost);
            Controls.Add(statusPanel);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FrmStation";
            Text = "Station";
            statusPanel.ResumeLayout(false);
            statusPanel.PerformLayout();
            stationPanelHost.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}
