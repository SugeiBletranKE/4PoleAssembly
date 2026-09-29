namespace FourAssembly.Forms;

public partial class AdminUnlockForm : Form
{
    private readonly string _expectedPassword = string.Empty;

    public bool Authorized { get; private set; }
    public int FailedAttempts { get; private set; }

    public AdminUnlockForm()
    {
        InitializeComponent();
    }

    public AdminUnlockForm(string stationName, string errorMessage, string expectedPassword)
    {
        _expectedPassword = expectedPassword;
        InitializeComponent();
        lblTitle.Text = $"{stationName} BLOQUEADA";
        lblError.Text = errorMessage;
    }

    private void AdminUnlockForm_Shown(object sender, EventArgs e) => FocusPasswordInput();

    private void AdminUnlockForm_Activated(object sender, EventArgs e) => FocusPasswordInput();

    private void BtnUnlock_Click(object sender, EventArgs e) => TryAuthorize();

    private void TxtPassword_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode != Keys.Enter && e.KeyCode != Keys.Tab)
            return;

        e.SuppressKeyPress = true;
        TryAuthorize();
    }

    private void FocusPasswordInput()
    {
        if (!txtPassword.CanFocus)
            return;

        txtPassword.Focus();
        txtPassword.SelectionStart = txtPassword.TextLength;
    }

    private void TryAuthorize()
    {
        if (string.IsNullOrEmpty(_expectedPassword))
        {
            lblStatus.Text = "AdminPass no esta configurada en settings.json.";
            return;
        }

        var scannedPassword = txtPassword.Text.TrimEnd('\r', '\n', '\t');
        if (!string.Equals(scannedPassword, _expectedPassword, StringComparison.Ordinal))
        {
            FailedAttempts++;
            lblStatus.Text = "Código de autorización incorrecto. Vuelva a escanear.";
            txtPassword.Clear();
            FocusPasswordInput();
            return;
        }

        Authorized = true;
        DialogResult = DialogResult.OK;
        Close();
    }
}
