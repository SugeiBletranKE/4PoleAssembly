namespace FourAssembly.MultiStation;

public partial class MultiStationMonitor : UserControl
{
    public MultiStationMonitor()
    {
        SetupUI();
    }

    private void SetupUI()
    {
        this.AutoScaleMode = AutoScaleMode.Font;
        this.BackColor = Color.White;

        var lblTitle = new Label
        {
            Text = "Station Monitor",
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            Location = new Point(20, 20),
            AutoSize = true
        };
        this.Controls.Add(lblTitle);

        var lblInfo = new Label
        {
            Text = "Multi-Monitor Station Control",
            Location = new Point(20, 60),
            AutoSize = true
        };
        this.Controls.Add(lblInfo);

        // TODO: Detectar monitores y crear formularios por estación
    }
}
