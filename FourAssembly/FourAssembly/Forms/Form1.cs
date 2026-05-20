namespace FourAssembly
{
    using FourAssembly.Controls;
    using FourAssembly.Forms;

    public partial class Form1 : Form
    {
        private StationControl[] _stations = null!;

        public Form1()
        {
            InitializeComponent();
            BuildStations();
            WireEvents();
        }

        private void BuildStations()
        {
            _stations = new StationControl[3];
            var pages = new[] { tabPage1, tabPage2, tabPage3 };
            for (int i = 0; i < 3; i++)
            {
                var sc = new StationControl { StationNumber = i + 1, Dock = DockStyle.Fill };
                pages[i].Controls.Add(sc);
                _stations[i] = sc;
            }
        }

        private void WireEvents()
        {
            btnRecipeEditor.Click += (_, _) =>
            {
                using var form = new RecipeEditorForm();
                form.ShowDialog(this);
            };
        }

        private void TabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var idx = tabControl1.SelectedIndex;
            if (idx >= 0 && idx < _stations.Length)
                _stations[idx].FocusScanInput();
        }
    }
}
