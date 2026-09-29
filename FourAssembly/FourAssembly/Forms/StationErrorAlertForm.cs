namespace FourAssembly.Forms;

public partial class StationErrorAlertForm : Form
{
    private readonly string _expectedPassword = string.Empty;

    public bool Authorized { get; private set; }
    public int FailedAttempts { get; private set; }

    public StationErrorAlertForm()
    {
        InitializeComponent();
    }

    public StationErrorAlertForm(string stationName, string errorCode, string errorMessage, string expectedPassword)
    {
        _expectedPassword = expectedPassword;
        InitializeComponent();
        lblStation.Text = stationName;
        lblErrorType.Text = errorMessage;
        lblErrorCode.Text = errorCode;
        lblStatus.Text = "Estación bloqueada - esperando autorización de Calidad";
    }

    private void StationErrorAlertForm_Shown(object sender, EventArgs e) => FocusAuthorizationInput();

    private void StationErrorAlertForm_Activated(object sender, EventArgs e) => FocusAuthorizationInput();

    private void btnAuthorize_Click(object sender, EventArgs e) => TryAuthorize();

    private void txtAuthorization_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode != Keys.Enter && e.KeyCode != Keys.Tab)
            return;

        e.SuppressKeyPress = true;
        TryAuthorize();
    }

    private void FocusAuthorizationInput()
    {
        if (!txtAuthorization.CanFocus)
            return;

        txtAuthorization.Focus();
        txtAuthorization.SelectionStart = txtAuthorization.TextLength;
    }

    private void TryAuthorize()
    {
        if (string.IsNullOrEmpty(_expectedPassword))
        {
            lblStatus.Text = "AdminPass no está configurada en settings.json.";
            return;
        }

        var scannedPassword = txtAuthorization.Text.TrimEnd('\r', '\n', '\t');
        if (!string.Equals(scannedPassword, _expectedPassword, StringComparison.Ordinal))
        {
            FailedAttempts++;
            lblStatus.Text = "Código de autorización incorrecto. Vuelva a escanear.";
            txtAuthorization.Clear();
            FocusAuthorizationInput();
            return;
        }

        Authorized = true;
        DialogResult = DialogResult.OK;
        Close();
    }

    private void StationErrorAlertForm_Load(object sender, EventArgs e)
    {

    }
}
