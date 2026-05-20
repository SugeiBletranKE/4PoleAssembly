namespace FourAssembly.Forms;

using FourAssembly.Models;
using FourAssembly.Services;

public partial class RecipeEditorForm : Form
{
    private Recipe? _editing;

    public RecipeEditorForm()
    {
        InitializeComponent();
        ConfigureGrids();
        RefreshList();
    }

    private void ConfigureGrids()
    {
        dgvMaterials.Columns.Clear();
        dgvMaterials.Columns.Add("Name", "Material Name");
        dgvMaterials.Columns.Add("Barcode", "Barcode");
        dgvMaterials.AllowUserToAddRows = true;
        dgvMaterials.AllowUserToDeleteRows = true;

        dgvTools.Columns.Clear();
        dgvTools.Columns.Add("Name", "Tool Name");
        dgvTools.Columns.Add("Barcode", "Barcode");
        dgvTools.AllowUserToAddRows = true;
        dgvTools.AllowUserToDeleteRows = true;
    }

    private void RefreshList()
    {
        lstRecipes.DataSource = null;
        lstRecipes.DataSource = RecipeRepository.GetAll()
            .Select(r => r.PartNumber)
            .ToList();
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

        dgvMaterials.Rows.Clear();
        foreach (var m in r?.Materials ?? [])
            dgvMaterials.Rows.Add(m.Name, m.Barcode);

        dgvTools.Rows.Clear();
        foreach (var t in r?.Tools ?? [])
            dgvTools.Rows.Add(t.Name, t.Barcode);
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

        var recipe = new Recipe { PartNumber = pn };

        foreach (DataGridViewRow row in dgvMaterials.Rows)
        {
            if (row.IsNewRow) continue;
            recipe.Materials.Add(new Material
            {
                Name = row.Cells["Name"].Value?.ToString() ?? string.Empty,
                Barcode = row.Cells["Barcode"].Value?.ToString() ?? string.Empty,
            });
        }

        foreach (DataGridViewRow row in dgvTools.Rows)
        {
            if (row.IsNewRow) continue;
            recipe.Tools.Add(new Tool
            {
                Name = row.Cells["Name"].Value?.ToString() ?? string.Empty,
                Barcode = row.Cells["Barcode"].Value?.ToString() ?? string.Empty,
            });
        }

        RecipeRepository.Upsert(recipe);
        RefreshList();
        MessageBox.Show($"Recipe '{pn}' saved.");
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
}
