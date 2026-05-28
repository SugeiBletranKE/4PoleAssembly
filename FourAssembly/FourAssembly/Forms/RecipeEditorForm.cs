namespace FourAssembly.Forms;

using FourAssembly.Models;
using FourAssembly.Services;
using System.Collections.Generic;

public partial class RecipeEditorForm : Form
{
    private Recipe? _editing;
    private List<Material> _currentMaterials = [];
    private List<Tool> _currentTools = [];
    private Dictionary<int, RecipeStationPanel> _stationPanels = [];
    private List<TextBox> _bgTextboxes = [];
    private int _selectedBGCount = 0;

    public RecipeEditorForm()
    {
        InitializeComponent();
        RefreshList();
        LoadRecipeIntoForm(null);
    }

    private void RefreshList()
    {
        lstRecipes.DataSource = null;
        lstRecipes.DataSource = RecipeRepository.GetAll()
            .Select(r => r.PartNumber)
            .ToList();
        lstRecipes.ClearSelected();
    }

    private void LstRecipes_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (lstRecipes.SelectedItem is string pn)
            LoadRecipeIntoForm(RecipeRepository.FindByPartNumber(pn));
    }

    private void LoadRecipeIntoForm(Recipe? r)
    {
        _editing = r;
        txtPartNumber.Text = r?.PartNumber ?? string.Empty;
        txtPartNumber.ReadOnly = r is not null;

        txtDescription.Text = r?.Description ?? string.Empty;
        txtDescription.ReadOnly = r is not null;

        var stationCount = r?.Stations.Count ?? 0;
        if (stationCount == 1) radBG1.Checked = true;
        else if (stationCount == 2) radBG2.Checked = true;
        else if (stationCount == 3) radBG3.Checked = true;
        else if (stationCount == 4) radBG4.Checked = true;

        _currentMaterials = new List<Material>(r?.Materials ?? []);
        _currentTools = new List<Tool>(r?.Tools ?? []);

        RefreshMaterialsList();
        RefreshToolsList();
        RefreshBGInputs();

        ClearMaterialInputs();
        ClearToolInputs();
    }

    private void RadBG_CheckedChanged(object? sender, EventArgs e)
    {
        if (radBG1.Checked) _selectedBGCount = 1;
        else if (radBG2.Checked) _selectedBGCount = 2;
        else if (radBG3.Checked) _selectedBGCount = 3;
        else if (radBG4.Checked) _selectedBGCount = 4;

        RefreshBGInputs();
    }

    private void RefreshBGInputs()
    {
        pnlBGInputs.Controls.Clear();
        _bgTextboxes.Clear();

        int xPos = 0;
        for (int i = 0; i < _selectedBGCount; i++)
        {
            var lbl = new Label { Text = $"BG {i + 1}:", Location = new Point(xPos, 5), AutoSize = true };
            pnlBGInputs.Controls.Add(lbl);

            var txt = new TextBox { Location = new Point(xPos + 50, 2), Size = new Size(100, 24) };
            if (_editing?.Stations.Count > i)
            {
                int stationNum = _selectedBGCount switch
                {
                    1 => 3,
                    2 => i == 0 ? 3 : 4,
                    3 => 2 + i,
                    4 => 1 + i,
                    _ => 0
                };
                var key = stationNum.ToString();
                if (_editing.Stations.ContainsKey(key))
                    txt.Text = _editing.Stations[key].BG;
            }
            pnlBGInputs.Controls.Add(txt);
            _bgTextboxes.Add(txt);

            xPos += 160;
        }

        pnlBGInputs.Height = _selectedBGCount > 0 ? 30 : 0;
        RefreshStationPanels();
    }

    private void RefreshStationPanels()
    {
        stationPanelsContainer.Controls.Clear();
        _stationPanels.Clear();

        int yPos = 10;
        int[] stationNumbers = _selectedBGCount switch
        {
            1 => [3],
            2 => [3, 4],
            3 => [2, 3, 4],
            4 => [1, 2, 3, 4],
            _ => []
        };

        for (int i = 0; i < stationNumbers.Length; i++)
        {
            int stationNum = stationNumbers[i];
            var stationKey = stationNum.ToString();
            var stationConfig = _editing?.Stations.ContainsKey(stationKey) == true
                ? _editing.Stations[stationKey]
                : new StationConfig { Name = stationNum == 4 ? "EOL" : $"Station {stationNum:D2}" };

            var panel = new RecipeStationPanel(stationNum, stationConfig, isFirstStation: i == 0);
            panel.Location = new Point(10, yPos);
            stationPanelsContainer.Controls.Add(panel);
            _stationPanels[stationNum] = panel;

            yPos += 120;
        }
    }

    private void RefreshMaterialsList()
    {
        lstMaterials.Items.Clear();
        foreach (var m in _currentMaterials)
            lstMaterials.Items.Add($"{m.Name} | {m.Barcode}");
    }

    private void RefreshToolsList()
    {
        lstTools.Items.Clear();
        foreach (var t in _currentTools)
            lstTools.Items.Add($"{t.Name} | {t.Barcode}");
    }

    private void BtnAddMaterial_Click(object sender, EventArgs e)
    {
        var name = txtMaterialName.Text.Trim();
        var barcode = txtMaterialBarcode.Text.Trim();

        if (string.IsNullOrEmpty(name))
        {
            MessageBox.Show("Material name is required.");
            return;
        }

        _currentMaterials.Add(new Material { Name = name, Barcode = barcode });
        RefreshMaterialsList();
        ClearMaterialInputs();
    }

    private void BtnRemoveMaterial_Click(object sender, EventArgs e)
    {
        if (lstMaterials.SelectedIndex < 0) return;
        _currentMaterials.RemoveAt(lstMaterials.SelectedIndex);
        RefreshMaterialsList();
    }

    private void BtnAddTool_Click(object sender, EventArgs e)
    {
        var name = txtToolName.Text.Trim();
        var barcode = txtToolBarcode.Text.Trim();

        if (string.IsNullOrEmpty(name))
        {
            MessageBox.Show("Tool name is required.");
            return;
        }

        _currentTools.Add(new Tool { Name = name, Barcode = barcode });
        RefreshToolsList();
        ClearToolInputs();
    }

    private void BtnRemoveTool_Click(object sender, EventArgs e)
    {
        if (lstTools.SelectedIndex < 0) return;
        _currentTools.RemoveAt(lstTools.SelectedIndex);
        RefreshToolsList();
    }

    private void ClearMaterialInputs()
    {
        txtMaterialName.Clear();
        txtMaterialBarcode.Clear();
        txtMaterialName.Focus();
    }

    private void ClearToolInputs()
    {
        txtToolName.Clear();
        txtToolBarcode.Clear();
        txtToolName.Focus();
    }

    private void BtnNew_Click(object sender, EventArgs e)
    {
        lstRecipes.ClearSelected();
        LoadRecipeIntoForm(null);
        txtPartNumber.ReadOnly = false;
        txtDescription.ReadOnly = false;
        txtPartNumber.Focus();
    }

    private void BtnSave_Click(object sender, EventArgs e)
    {
        var pn = txtPartNumber.Text.Trim();
        if (string.IsNullOrEmpty(pn))
        {
            MessageBox.Show("Part number is required.");
            return;
        }

        if (_selectedBGCount == 0)
        {
            MessageBox.Show("Please select number of BGs.");
            return;
        }

        var recipe = new Recipe
        {
            PartNumber = pn,
            Description = txtDescription.Text.Trim(),
            Materials = _currentMaterials,
            Tools = _currentTools
        };

        int[] stationNumbers = _selectedBGCount switch
        {
            1 => [3],
            2 => [3, 4],
            3 => [2, 3, 4],
            4 => [1, 2, 3, 4],
            _ => []
        };

        for (int i = 0; i < stationNumbers.Length; i++)
        {
            int stationNum = stationNumbers[i];
            var stationKey = stationNum.ToString();
            var stationConfig = _stationPanels[stationNum].GetStationConfig();

            if (i == 0 && _selectedBGCount > 0)
                stationConfig.HousingEnable = stationConfig.HousingEnable;
            else
                stationConfig.HousingEnable = "FALSE";

            if (!string.IsNullOrEmpty(_bgTextboxes[i].Text))
                stationConfig.BG = _bgTextboxes[i].Text.Trim();

            recipe.Stations[stationKey] = stationConfig;
        }

        RecipeRepository.Upsert(recipe);
        RefreshList();
        MessageBox.Show($"Recipe '{pn}' saved.");
        LoadRecipeIntoForm(null);
        lstRecipes.ClearSelected();
    }

    private void BtnDelete_Click(object sender, EventArgs e)
    {
        if (lstRecipes.SelectedItem is not string pn) return;
        if (MessageBox.Show($"Delete '{pn}'?", "Confirm",
            MessageBoxButtons.YesNo) != DialogResult.Yes) return;
        RecipeRepository.Delete(pn);
        RefreshList();
        LoadRecipeIntoForm(null);
    }

    private void BtnReset_Click(object sender, EventArgs e)
    {
        LoadRecipeIntoForm(null);
        txtPartNumber.ReadOnly = false;
        txtDescription.ReadOnly = false;
        lstRecipes.ClearSelected();
    }
}

public class RecipeStationPanel : Panel
{
    private int _stationNum;
    private bool _isFirstStation;
    private TextBox txtHousing = null!;
    private CheckBox chkHousingEnable = null!;

    public RecipeStationPanel(int stationNum, StationConfig config, bool isFirstStation = false)
    {
        _stationNum = stationNum;
        _isFirstStation = isFirstStation;
        InitializeControls(config);
    }

    private void InitializeControls(StationConfig config)
    {
        this.Size = new Size(720, 80);
        this.BorderStyle = BorderStyle.FixedSingle;
        this.BackColor = Color.WhiteSmoke;

        var lblStation = new Label
        {
            Text = $"Station {_stationNum:D2}",
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            Location = new Point(10, 10),
            AutoSize = true
        };
        this.Controls.Add(lblStation);

        if (_isFirstStation)
        {
            chkHousingEnable = new CheckBox { Text = "Housing Enable", Location = new Point(10, 35), Checked = config.HousingEnable == "TRUE", AutoSize = true };
            this.Controls.Add(chkHousingEnable);
        }

        var lblHousing = new Label { Text = "Housing:", Location = new Point(200, 35), AutoSize = true };
        txtHousing = new TextBox { Location = new Point(270, 33), Size = new Size(120, 24), Text = config.Housing };
        this.Controls.Add(lblHousing);
        this.Controls.Add(txtHousing);
    }

    public StationConfig GetStationConfig() => new()
    {
        Name = $"Station{_stationNum:D2}",
        Enable = "TRUE",
        BG = string.Empty,
        HousingEnable = _isFirstStation && chkHousingEnable?.Checked == true ? "TRUE" : "FALSE",
        Housing = txtHousing.Text.Trim()
    };
}
