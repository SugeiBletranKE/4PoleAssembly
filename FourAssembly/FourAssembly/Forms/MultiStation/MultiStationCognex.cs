namespace FourAssembly.MultiStation;

public partial class MultiStationCognex : UserControl
{
    public MultiStationCognex()
    {
        SetupUI();
    }

    private void SetupUI()
    {
        this.AutoScaleMode = AutoScaleMode.Font;
        this.BackColor = Color.White;

        var lblTitle = new Label
        {
            Text = "Cognex Diagnostics",
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            Location = new Point(20, 20),
            AutoSize = true
        };
        this.Controls.Add(lblTitle);

        var lblInfo = new Label
        {
            Text = "Multi-Station Cognex Monitoring",
            Location = new Point(20, 60),
            AutoSize = true
        };
        this.Controls.Add(lblInfo);

        // TODO: Implementar diagnostics para múltiples estaciones
    }
}
