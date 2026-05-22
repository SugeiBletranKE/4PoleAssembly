namespace FourAssembly.MultiStation;

using FourAssembly.Services;

public partial class MultiStationRecipes : UserControl
{
    public MultiStationRecipes()
    {
        SetupUI();
    }

    private void SetupUI()
    {
        this.AutoScaleMode = AutoScaleMode.Font;
        this.BackColor = Color.White;

        var lblTitle = new Label
        {
            Text = "Recipes Management",
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            Location = new Point(20, 20),
            AutoSize = true
        };
        this.Controls.Add(lblTitle);

        var lblInfo = new Label
        {
            Text = "Multi-Station Recipe Selection",
            Location = new Point(20, 60),
            AutoSize = true
        };
        this.Controls.Add(lblInfo);

        // TODO: Implementar lista de recetas para multi-estación
    }
}
