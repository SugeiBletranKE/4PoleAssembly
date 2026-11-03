namespace FourAssembly.Forms;

using FourAssembly.Models;
using FourAssembly.Services;
using System.Collections.Generic;

public partial class RecipeEditorForm : Form
{
    private Recipe? _editing;
    private List<Tool> _currentTools = [];
    private Dictionary<int, RecipeStationPanel> _stationPanels = [];
    private List<TextBox> _bgTextboxes = [];
    private readonly Dictionary<int, TextBox> _stationToolBarcodeTextboxes = [];
    private readonly Panel _stationToolsPanel = new();
    private readonly Panel _housingPanel = new();
    private readonly Panel _bgPinsPanel = new();
    private readonly Dictionary<(int Station, int Pin), CheckBox> _bgPinCheckBoxes = [];
    private readonly TextBox _txtHousing = new();
    private int _selectedBGCount = 0;
    private bool _loadingRecipe;

    public RecipeEditorForm()
    {
        InitializeComponent();
        lblDescription.Visible = false;
        txtDescription.Visible = false;
        lblBGs.Text = "Number of cables";
        radBG1.Text = "1 cable";
        radBG2.Text = "2 cables";
        radBG3.Text = "3 cables";
        radBG4.Text = "4 cables";
        GroupMaterialStationRadioButtons();
        HideLegacyMaterialControls();
        SetupHousingPanel();
        SetupBgPinsPanel();
        SetupStationToolsPanel();
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
        _loadingRecipe = true;
        _editing = r;
        txtPartNumber.Text = r?.PartNumber ?? string.Empty;
        txtPartNumber.ReadOnly = r is not null;
        _chkSharedMode.Checked = r?.SharedMode == true;

        SetConnectorGenderSelection(r?.ConnectorGender ?? "A");

        var bgCount = GetBGCountFromRecipe(r);
        _selectedBGCount = bgCount;

        radBG1.Checked = false;
        radBG2.Checked = false;
        radBG3.Checked = false;
        radBG4.Checked = false;

        if (bgCount == 1) radBG1.Checked = true;
        else if (bgCount == 2) radBG2.Checked = true;
        else if (bgCount == 3) radBG3.Checked = true;
        else if (bgCount == 4) radBG4.Checked = true;
        _selectedBGCount = bgCount;

        _currentTools = new List<Tool>(r?.Tools ?? []);

        RefreshToolsList();
        RefreshBGInputs();
        RefreshHousingInputs();
        RefreshBgPinInputs();
        _loadingRecipe = false;
    }

    private void SharedMode_CheckedChanged(object? sender, EventArgs e)
    {
        if (_loadingRecipe)
            return;

        RefreshBGInputs();
    }

    private void RadBG_CheckedChanged(object? sender, EventArgs e)
    {
        if (sender is RadioButton radio && !radio.Checked)
            return;

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
        var stationNumbers = GetRecipeStationNumbers();
        var bgInputCount = _chkSharedMode.Checked && stationNumbers.Length > 0
            ? 1
            : stationNumbers.Length;

        for (int i = 0; i < bgInputCount; i++)
        {
            var lbl = new Label
            {
                Text = _chkSharedMode.Checked ? "BG compartido:" : $"BG {i + 1}:",
                Location = new Point(xPos, 5),
                AutoSize = true
            };
            pnlBGInputs.Controls.Add(lbl);

            var textOffset = _chkSharedMode.Checked ? 115 : 50;
            var txt = new TextBox { Location = new Point(xPos + textOffset, 2), Size = new Size(150, 24) };
            if (_editing is not null)
            {
                if (_chkSharedMode.Checked)
                {
                    txt.Text = GetSharedBgFromRecipe(_editing);
                }
                else
                {
                    var key = stationNumbers[i].ToString();
                    if (_editing.Stations.ContainsKey(key))
                        txt.Text = _editing.Stations[key].BG;
                }
            }
            pnlBGInputs.Controls.Add(txt);
            _bgTextboxes.Add(txt);

            xPos += 160;
        }

        pnlBGInputs.Height = _selectedBGCount > 0 ? 30 : 0;
        RefreshStationPanels();
        RefreshHousingInputs();
        RefreshBgPinInputs();
        RefreshStationToolInputs();
    }

    private void RefreshStationPanels()
    {
        /*stationPanelsContainer.Controls.Clear();
        _stationPanels.Clear();

        int yPos = 10;
        int[] stationNumbers = GetStationNumbersForBGCount(_selectedBGCount);

        for (int i = 0; i < stationNumbers.Length; i++)
        {
            int stationNum = stationNumbers[i];
            var stationKey = stationNum.ToString();
            var stationConfig = _editing?.Stations.ContainsKey(stationKey) == true
                ? _editing.Stations[stationKey]
                : new StationConfig { Name = $"Station {stationNum:D2}" };

            var panel = new RecipeStationPanel(stationNum, stationConfig, isFirstStation: i == 0);
            panel.Location = new Point(10, yPos);
            stationPanelsContainer.Controls.Add(panel);
            _stationPanels[stationNum] = panel;

            yPos += panel.Height + 10;
        }*/
    }

    private void RefreshMaterialsList()
    {
        lstMaterials.Items.Clear();
    }

    private void RefreshToolsList()
    {
        lstTools.Items.Clear();
        foreach (var t in _currentTools)
            lstTools.Items.Add($"{t.Name} | {t.Barcode}");
    }

    private void SetupStationToolsPanel()
    {
        lblTools.Text = "Tool barcode by station";
        HideLegacyToolControls();

        _stationToolsPanel.Location = new Point(15, 448);
        _stationToolsPanel.Size = new Size(720, 120);
        _stationToolsPanel.AutoSize = true;
        scrollPanel.Controls.Add(_stationToolsPanel);

        btnSave.Location = new Point(5, 610);
        btnReset.Location = new Point(115, 610);
    }

    private void SetupHousingPanel()
    {
        _housingPanel.Location = new Point(15, 232);
        _housingPanel.Size = new Size(360, 55);

        var lblHousing = new Label
        {
            Text = "Material / Housing barcode:",
            Location = new Point(0, 0),
            AutoSize = true,
            Font = new Font("Segoe UI", 11F, FontStyle.Bold)
        };

        _txtHousing.Location = new Point(0, 23);
        _txtHousing.Size = new Size(360, 25);

        _housingPanel.Controls.Add(lblHousing);
        _housingPanel.Controls.Add(_txtHousing);
        scrollPanel.Controls.Add(_housingPanel);
    }

    private void SetupBgPinsPanel()
    {
        _bgPinsPanel.Location = new Point(15, 295);
        _bgPinsPanel.Size = new Size(720, 140);
        scrollPanel.Controls.Add(_bgPinsPanel);
    }

    private void RefreshBgPinInputs()
    {
        _bgPinsPanel.Controls.Clear();
        _bgPinCheckBoxes.Clear();

        var stationNumbers = GetRecipeStationNumbers();
        _bgPinsPanel.Visible = stationNumbers.Length > 0;
        if (stationNumbers.Length == 0)
            return;

        var sharedPins = _chkSharedMode.Checked;
        var pinRows = sharedPins ? new[] { 0 } : stationNumbers;

        _bgPinsPanel.Controls.Add(new Label
        {
            Text = sharedPins
                ? "BG pins compartidos (se aplican igual a las 3 estaciones)"
                : "BG pins por estacion (1 = habilitado, 0 = deshabilitado)",
            Location = new Point(0, 0),
            AutoSize = true,
            Font = new Font("Segoe UI", 11F, FontStyle.Bold)
        });

        for (var pinNumber = 1; pinNumber <= 4; pinNumber++)
        {
            _bgPinsPanel.Controls.Add(new Label
            {
                Text = $"Pin {pinNumber}",
                Location = new Point(120 + ((pinNumber - 1) * 100), 32),
                Size = new Size(95, 22),
                TextAlign = ContentAlignment.MiddleCenter
            });

        }

        for (var row = 0; row < pinRows.Length; row++)
        {
            var stationNumber = pinRows[row];
            var y = 58 + (row * 30);
            _bgPinsPanel.Controls.Add(new Label
            {
                Text = sharedPins ? "Compartido" : $"Station {stationNumber}",
                Location = new Point(0, y + 2),
                Size = new Size(100, 22)
            });

            var savedPins = sharedPins
                ? GetSavedSharedBgPins()
                : _editing?.Stations.TryGetValue(stationNumber.ToString(), out var station) == true
                    ? station.BgPins
                    : null;

            for (var pinNumber = 1; pinNumber <= 4; pinNumber++)
            {
                var isEnabled = savedPins is { Count: >= 4 }
                    && savedPins[pinNumber - 1] == 1;
                var checkBox = new CheckBox
                {
                    Checked = isEnabled,
                    Location = new Point(158 + ((pinNumber - 1) * 100), y),
                    AutoSize = true,
                    Text = isEnabled ? "1" : "0"
                };
                checkBox.CheckedChanged += (_, _) => checkBox.Text = checkBox.Checked ? "1" : "0";
                _bgPinsPanel.Controls.Add(checkBox);
                _bgPinCheckBoxes[(stationNumber, pinNumber)] = checkBox;
            }
        }

        _bgPinsPanel.Height = 62 + (pinRows.Length * 30);
    }

    private List<int>? GetSavedSharedBgPins()
    {
        if (_editing?.Stations is null)
            return null;

        // Compatibilidad con recetas compartidas anteriores: toma primero el
        // patron configurado que tenga al menos un pin activo.
        return _editing.Stations
            .OrderBy(entry => int.TryParse(entry.Key, out var number) ? number : int.MaxValue)
            .Select(entry => entry.Value.BgPins)
            .FirstOrDefault(pins => pins is { Count: >= 4 } && pins.Take(4).Any(value => value == 1))
            ?? _editing.Stations.Values
                .Select(station => station.BgPins)
                .FirstOrDefault(pins => pins is { Count: >= 4 });
    }

    private void RefreshHousingInputs()
    {
        var stationNumbers = GetRecipeStationNumbers();
        if (stationNumbers.Length == 0)
        {
            _housingPanel.Visible = false;
            _txtHousing.Clear();
            return;
        }

        _housingPanel.Visible = true;
        var firstStationKey = stationNumbers[0].ToString();
        if (_editing?.Stations.TryGetValue(firstStationKey, out var firstStation) == true)
        {
            _txtHousing.Text = firstStation.Housing;
            return;
        }

        _txtHousing.Clear();
    }

    private void HideLegacyToolControls()
    {
        txtToolName.Visible = false;
        txtToolBarcode.Visible = false;
        btnAddTool.Visible = false;
        lstTools.Visible = false;
        btnRemoveTool.Visible = false;
    }

    private void HideLegacyMaterialControls()
    {
        lblMaterials.Visible = false;
        txtMaterialName.Visible = false;
        txtMaterialBarcode.Visible = false;
        btnAddMaterial.Visible = false;
        lstMaterials.Visible = false;
        btnRemoveMaterial.Visible = false;

        if (label2.Parent is not null)
            label2.Parent.Visible = false;
        else
            label2.Visible = false;

        rdb01.Visible = false;
        rdb02.Visible = false;
        rdb03.Visible = false;
    }

    private void RefreshStationToolInputs()
    {
        _stationToolsPanel.Top = _bgPinsPanel.Visible
            ? _bgPinsPanel.Bottom + 38
            : 448;
        lblTools.Top = _stationToolsPanel.Top - 25;
        _stationToolsPanel.Controls.Clear();
        _stationToolBarcodeTextboxes.Clear();

        var stationNumbers = GetRecipeStationNumbers();
        for (int i = 0; i < stationNumbers.Length; i++)
        {
            var stationNum = stationNumbers[i];
            var tool = i < _currentTools.Count ? _currentTools[i] : null;
            var y = i * 32;

            var lblStation = new Label
            {
                Text = $"Station {stationNum}",
                Location = new Point(0, y + 5),
                Size = new Size(85, 24)
            };

            var txtBarcode = new TextBox
            {
                Location = new Point(95, y + 2),
                Size = new Size(460, 25),
                PlaceholderText = "Escanee Tool barcode",
                Text = !string.IsNullOrWhiteSpace(tool?.Barcode) ? tool.Barcode : tool?.Name ?? string.Empty
            };

            _stationToolsPanel.Controls.Add(lblStation);
            _stationToolsPanel.Controls.Add(txtBarcode);
            _stationToolBarcodeTextboxes[stationNum] = txtBarcode;
        }

        var panelHeight = Math.Max(32, stationNumbers.Length * 32);
        _stationToolsPanel.Height = panelHeight;
        btnSave.Top = _stationToolsPanel.Bottom + 18;
        btnReset.Top = btnSave.Top;
    }

    private List<Tool> GetToolsByStationFromInputs()
    {
        var tools = new List<Tool>();
        foreach (var stationNum in GetRecipeStationNumbers())
        {
            var barcode = _stationToolBarcodeTextboxes.TryGetValue(stationNum, out var barcodeTextBox)
                ? barcodeTextBox.Text.Trim()
                : string.Empty;

            // Name se conserva por compatibilidad, usando el mismo valor escaneado.
            tools.Add(new Tool { Name = barcode, Barcode = barcode });
        }

        return tools;
    }

    private void BtnAddMaterial_Click(object sender, EventArgs e)
    {
        RefreshMaterialsList();
        ClearMaterialInputs();
    }

    private void BtnRemoveMaterial_Click(object sender, EventArgs e)
    {
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
            MessageBox.Show("Please select number of cables.");
            return;
        }

        if (_chkSharedMode.Checked
            && (_bgTextboxes.Count == 0 || string.IsNullOrWhiteSpace(_bgTextboxes[0].Text)))
        {
            MessageBox.Show("Shared BG is required.");
            return;
        }

        if (GetRecipeStationNumbers().Length > 0
            && string.IsNullOrWhiteSpace(_txtHousing.Text))
        {
            MessageBox.Show("Housing is required.");
            return;
        }

        var recipe = new Recipe
        {
            PartNumber = pn,
            Description = string.Empty,
            CableCount = _selectedBGCount,
            SharedMode = _chkSharedMode.Checked,
            SharedBG = _chkSharedMode.Checked && _bgTextboxes.Count > 0
                ? _bgTextboxes[0].Text.Trim()
                : string.Empty,
            // Cambio solicitado: la estacion del conector/material se calcula segun cantidad de BGs.
            ConnectorGender = GetSelectedConnectorGender(),
            Tools = GetToolsByStationFromInputs()
        };

        int[] stationNumbers = GetRecipeStationNumbers();
        var sharedBgPins = _chkSharedMode.Checked
            ? Enumerable.Range(1, 4)
                .Select(pin => _bgPinCheckBoxes.TryGetValue((0, pin), out var checkBox) && checkBox.Checked ? 1 : 0)
                .ToList()
            : null;

        for (int i = 0; i < stationNumbers.Length; i++)
        {
            int stationNum = stationNumbers[i];
            var stationKey = stationNum.ToString();
            var stationConfig = _editing?.Stations.TryGetValue(stationKey, out var existingConfig) == true
                ? existingConfig
                : new StationConfig();

            var bgTextBoxIndex = _chkSharedMode.Checked ? 0 : i;
            if (bgTextBoxIndex < _bgTextboxes.Count
                && !string.IsNullOrWhiteSpace(_bgTextboxes[bgTextBoxIndex].Text))
            {
                stationConfig.BG = _bgTextboxes[bgTextBoxIndex].Text.Trim();
            }

            stationConfig.Enable = "TRUE";
            var housingEnabledForStation = _chkSharedMode.Checked || i == 0;
            stationConfig.HousingEnable = housingEnabledForStation ? "TRUE" : "FALSE";
            stationConfig.Housing = housingEnabledForStation
                ? _txtHousing.Text.Trim()
                : string.Empty;
            stationConfig.BgPins = sharedBgPins is not null
                ? new List<int>(sharedBgPins)
                : Enumerable.Range(1, 4)
                    .Select(pin => _bgPinCheckBoxes.TryGetValue((stationNum, pin), out var checkBox) && checkBox.Checked ? 1 : 0)
                    .ToList();
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
        lstRecipes.ClearSelected();
    }

    private static int[] GetStationNumbersForBGCount(int bgCount) => bgCount switch
    {
        // Un cable de cada variante pasa por una maquina externa.
        // El sistema propio procesa siempre la cantidad total menos uno.
        1 => [],
        2 => [3],
        3 => [2, 3],
        4 => [1, 2, 3],
        _ => []
    };

    private int[] GetRecipeStationNumbers()
    {
        return _chkSharedMode.Checked && _selectedBGCount > 0
            ? [1, 2, 3]
            : GetStationNumbersForBGCount(_selectedBGCount);
    }

    private static string GetSharedBgFromRecipe(Recipe recipe)
    {
        if (!string.IsNullOrWhiteSpace(recipe.SharedBG))
            return recipe.SharedBG.Trim();

        return recipe.Stations
            .Where(station => station.Value.Enable == "TRUE")
            .OrderBy(station => int.TryParse(station.Key, out var number) ? number : int.MaxValue)
            .Select(station => station.Value.BG?.Trim() ?? string.Empty)
            .FirstOrDefault(bg => !string.IsNullOrWhiteSpace(bg))
            ?? string.Empty;
    }

    private static string GetMaterialStationCodeForBGCount(int bgCount)
    {
        return GetStationNumbersForBGCount(bgCount).FirstOrDefault().ToString();
    }

    private static int GetBGCountFromRecipe(Recipe? recipe)
    {
        if (recipe is null)
            return 0;

        if (recipe.CableCount is >= 1 and <= 4)
            return recipe.CableCount;

        var enabled = recipe.Stations
            .Where(kvp => kvp.Value.Enable == "TRUE")
            .Select(kvp => kvp.Key)
            .ToHashSet();

        var enabledStationCount = enabled.Count(k =>
            int.TryParse(k, out var station) && station >= 1 && station <= 3);

        // Compatibilidad con recetas anteriores que no guardaban CableCount.
        return enabledStationCount == 0 ? 0 : Math.Min(4, enabledStationCount + 1);
    }

    private void EnsureMaterialStationControls()
    {
        var lblMaterialStation = new Label
        {
            Text = "Material en estación:",
            Location = new Point(430, 135),
            AutoSize = true,
            Font = new Font("Segoe UI", 11F, FontStyle.Bold)
        };

        rdb03 = new RadioButton
        {
            Name = "rdb03",
            Text = "03",
            Location = new Point(430, 160),
            AutoSize = true,
            Checked = true
        };

        rdb02 = new RadioButton
        {
            Name = "rdb02",
            Text = "02",
            Location = new Point(485, 160),
            AutoSize = true
        };

        rdb01 = new RadioButton
        {
            Name = "rdb01",
            Text = "01",
            Location = new Point(540, 160),
            AutoSize = true
        };

        scrollPanel.Controls.Add(lblMaterialStation);
        scrollPanel.Controls.Add(rdb03);
        scrollPanel.Controls.Add(rdb02);
        scrollPanel.Controls.Add(rdb01);
    }

    private string GetSelectedMaterialStationCode()
    {
        if (rdb01.Checked)
            return "1";

        if (rdb02.Checked)
            return "2";

        return "3";
    }

    private void SetMaterialStationSelection(string stationCode)
    {
        rdb01.Checked = stationCode == "1";
        rdb02.Checked = stationCode == "2";
        rdb03.Checked = stationCode != "1" && stationCode != "2";
    }

    private string GetSelectedConnectorGender()
    {
        return rdbConnectorMale.Checked ? "B" : "A";
    }

    private void SetConnectorGenderSelection(string connectorGender)
    {
        var isMale = string.Equals(connectorGender, "B", StringComparison.OrdinalIgnoreCase);
        rdbConnectorMale.Checked = isMale;
        rdbConnectorFemale.Checked = !isMale;
    }

    private void GroupMaterialStationRadioButtons()
    {
        var materialStationPanel = new Panel
        {
            Name = "pnlMaterialStation",
            Location = new Point(15, 232),
            Size = new Size(320, 55)
        };

        scrollPanel.Controls.Remove(label2);
        scrollPanel.Controls.Remove(rdb01);
        scrollPanel.Controls.Remove(rdb02);
        scrollPanel.Controls.Remove(rdb03);

        label2.Location = new Point(0, 0);
        rdb01.Location = new Point(0, 25);
        rdb02.Location = new Point(100, 25);
        rdb03.Location = new Point(196, 25);

        materialStationPanel.Controls.Add(label2);
        materialStationPanel.Controls.Add(rdb01);
        materialStationPanel.Controls.Add(rdb02);
        materialStationPanel.Controls.Add(rdb03);
        scrollPanel.Controls.Add(materialStationPanel);
    }
}

public class RecipeStationPanel : Panel
{
    private int _stationNum;
    private bool _isFirstStation;
    private TextBox? txtHousing;
    private CheckBox? chkHousingEnable;

    public RecipeStationPanel(int stationNum, StationConfig config, bool isFirstStation = false)
    {
        _stationNum = stationNum;
        _isFirstStation = isFirstStation;
        InitializeControls(config);
    }

    private void InitializeControls(StationConfig config)
    {
        this.Size = new Size(720, _isFirstStation ? 80 : 45);
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

            var lblHousing = new Label { Text = "Housing:", Location = new Point(200, 35), AutoSize = true };
            txtHousing = new TextBox { Location = new Point(270, 33), Size = new Size(120, 24), Text = config.Housing };
            this.Controls.Add(lblHousing);
            this.Controls.Add(txtHousing);
        }
    }

    public StationConfig GetStationConfig() => new()
    {
        Name = $"Station{_stationNum:D2}",
        Enable = "TRUE",
        BG = string.Empty,
        HousingEnable = _isFirstStation && chkHousingEnable?.Checked == true ? "TRUE" : "FALSE",
        Housing = _isFirstStation ? txtHousing?.Text.Trim() ?? string.Empty : string.Empty
    };
}

