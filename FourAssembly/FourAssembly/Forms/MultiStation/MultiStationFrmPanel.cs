namespace FourAssembly.MultiStation;

using FourAssembly.Services;

public partial class MultiStationFrmPanel : Form
{
    private TabPage tabRecipes = null!;
    private TabPage tabMonitor = null!;
    private TabPage tabCognex = null!;

    private MultiStationRecipes? recipesPanel;
    private MultiStationMonitor? monitorPanel;
    private MultiStationCognex? cognexPanel;

    public MultiStationFrmPanel()
    {
        this.Text = "Multi-Station System";
        this.Size = new Size(1024, 768);
        this.StartPosition = FormStartPosition.CenterScreen;
        InitializeTabs();
        RecipeRepository.Load();
    }

    private void InitializeTabs()
    {
        var tabControl = new TabControl
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(10)
        };

        // Tab de Recipes
        tabRecipes = new TabPage("Recipes");
        recipesPanel = new MultiStationRecipes();
        recipesPanel.Dock = DockStyle.Fill;
        tabRecipes.Controls.Add(recipesPanel);
        tabControl.TabPages.Add(tabRecipes);

        // Tab de Monitor
        tabMonitor = new TabPage("Station Monitor");
        monitorPanel = new MultiStationMonitor();
        monitorPanel.Dock = DockStyle.Fill;
        tabMonitor.Controls.Add(monitorPanel);
        tabControl.TabPages.Add(tabMonitor);

        // Tab de Cognex
        tabCognex = new TabPage("Cognex Diagnostics");
        cognexPanel = new MultiStationCognex();
        cognexPanel.Dock = DockStyle.Fill;
        tabCognex.Controls.Add(cognexPanel);
        tabControl.TabPages.Add(tabCognex);

        this.Controls.Add(tabControl);
    }
}
