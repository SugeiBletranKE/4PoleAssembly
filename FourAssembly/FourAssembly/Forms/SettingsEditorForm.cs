namespace FourAssembly.Forms;

using FourAssembly.Services;
using System.Text.Json;

public partial class SettingsEditorForm : Form
{
    private readonly Func<int?, Task>? _resetRequested;
    private static readonly JsonSerializerOptions _formatOptions = new()
    {
        WriteIndented = true,
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip
    };

    public SettingsEditorForm() : this(null)
    {
    }

    public SettingsEditorForm(Func<int?, Task>? resetRequested)
    {
        _resetRequested = resetRequested;
        InitializeComponent();
        lblPath.Text = SettingsManager.FilePath;
        LoadSettings();
    }

    private async void BtnResetGeneral_Click(object sender, EventArgs e) => await RequestResetAsync(null);

    private async void BtnResetStation1_Click(object sender, EventArgs e) => await RequestResetAsync(1);

    private async void BtnResetStation2_Click(object sender, EventArgs e) => await RequestResetAsync(2);

    private async void BtnResetStation3_Click(object sender, EventArgs e) => await RequestResetAsync(3);

    private async Task RequestResetAsync(int? stationNumber)
    {
        if (_resetRequested == null)
        {
            MessageBox.Show("El reset solamente está disponible al abrir Settings desde FrmMain.",
                "Reset", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        await _resetRequested(stationNumber);
    }

    private void LoadSettings()
    {
        try
        {
            txtJson.Text = SettingsManager.ReadRawJson();
            txtJson.SelectionStart = 0;
            txtJson.SelectionLength = 0;
            SetStatus("Configuracion cargada.", Color.DarkGreen);
        }
        catch (Exception ex)
        {
            SetStatus($"No se pudo cargar: {ex.Message}", Color.Firebrick);
        }
    }

    private void BtnReload_Click(object sender, EventArgs e)
    {
        if (txtJson.Modified
            && MessageBox.Show(
                "Se perderan los cambios no guardados. ¿Deseas recargar?",
                "Recargar settings",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning) != DialogResult.Yes)
            return;

        LoadSettings();
        txtJson.Modified = false;
    }

    private void BtnFormat_Click(object sender, EventArgs e)
    {
        try
        {
            var settings = JsonSerializer.Deserialize<AppSettings>(txtJson.Text, _formatOptions)
                ?? throw new JsonException("El contenido esta vacio.");
            txtJson.Text = JsonSerializer.Serialize(settings, _formatOptions);
            SetStatus("JSON valido y formateado. Aun no se ha guardado.", Color.DarkOrange);
        }
        catch (JsonException ex)
        {
            SetStatus($"JSON invalido: {ex.Message}", Color.Firebrick);
            MessageBox.Show(ex.Message, "JSON invalido", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnSave_Click(object sender, EventArgs e)
    {
        SaveSettings(
            "Se guardara settings.json y se creara settings.json.bak. " +
            "Los cambios se aplicaran al reiniciar el sistema. ¿Deseas continuar?");
    }

    private void BtnSaveAndRestart_Click(object sender, EventArgs e)
    {
        var saved = SaveSettings(
            "Se guardara settings.json y el sistema se cerrara y abrira nuevamente. " +
            "Asegurate de que no haya un proceso de produccion activo. ¿Deseas continuar?");

        if (!saved)
            return;

        Application.Restart();
    }

    private bool SaveSettings(string confirmationMessage)
    {
        try
        {
            _ = JsonSerializer.Deserialize<AppSettings>(txtJson.Text, _formatOptions)
                ?? throw new JsonException("El contenido esta vacio.");

            if (MessageBox.Show(
                confirmationMessage,
                "Guardar configuracion",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning) != DialogResult.Yes)
                return false;

            SettingsManager.SaveRawJson(txtJson.Text);
            txtJson.Modified = false;
            SetStatus("Guardado correctamente. Reinicia el sistema para aplicar los cambios.", Color.DarkGreen);
            return true;
        }
        catch (JsonException ex)
        {
            SetStatus($"No se guardo: JSON invalido. {ex.Message}", Color.Firebrick);
            MessageBox.Show(ex.Message, "JSON invalido", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }
        catch (Exception ex)
        {
            SetStatus($"No se pudo guardar: {ex.Message}", Color.Firebrick);
            MessageBox.Show(ex.Message, "Error al guardar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }
    }

    private void SetStatus(string message, Color color)
    {
        lblStatus.Text = message;
        lblStatus.ForeColor = color;
    }
}
