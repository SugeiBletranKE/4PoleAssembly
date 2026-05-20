namespace FourAssembly.Forms;

partial class FrmPanel
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        pnlMenu = new Panel();
        btnStations = new Button();
        btnRecipeEditor = new Button();
        btnCognexDiagnostics = new Button();
        pnlContent = new Panel();
        pnlMenu.SuspendLayout();
        SuspendLayout();

        // pnlMenu
        pnlMenu.BackColor = Color.DarkGray;
        pnlMenu.Controls.Add(btnCognexDiagnostics);
        pnlMenu.Controls.Add(btnRecipeEditor);
        pnlMenu.Controls.Add(btnStations);
        pnlMenu.Dock = DockStyle.Top;
        pnlMenu.Location = new Point(0, 0);
        pnlMenu.Name = "pnlMenu";
        pnlMenu.Size = new Size(1147, 60);
        pnlMenu.TabIndex = 0;

        // btnStations
        btnStations.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        btnStations.Location = new Point(12, 12);
        btnStations.Name = "btnStations";
        btnStations.Size = new Size(140, 36);
        btnStations.TabIndex = 0;
        btnStations.Text = "Estaciones";
        btnStations.UseVisualStyleBackColor = true;
        btnStations.Click += BtnStations_Click;

        // btnRecipeEditor
        btnRecipeEditor.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        btnRecipeEditor.Location = new Point(158, 12);
        btnRecipeEditor.Name = "btnRecipeEditor";
        btnRecipeEditor.Size = new Size(140, 36);
        btnRecipeEditor.TabIndex = 1;
        btnRecipeEditor.Text = "Editor Recetas";
        btnRecipeEditor.UseVisualStyleBackColor = true;
        btnRecipeEditor.Click += BtnRecipeEditor_Click;

        // btnCognexDiagnostics
        btnCognexDiagnostics.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        btnCognexDiagnostics.Location = new Point(304, 12);
        btnCognexDiagnostics.Name = "btnCognexDiagnostics";
        btnCognexDiagnostics.Size = new Size(140, 36);
        btnCognexDiagnostics.TabIndex = 2;
        btnCognexDiagnostics.Text = "Cognex Debug";
        btnCognexDiagnostics.UseVisualStyleBackColor = true;
        btnCognexDiagnostics.Click += BtnCognexDiagnostics_Click;

        // pnlContent
        pnlContent.Dock = DockStyle.Fill;
        pnlContent.Location = new Point(0, 60);
        pnlContent.Name = "pnlContent";
        pnlContent.Size = new Size(1147, 713);
        pnlContent.TabIndex = 1;

        // FrmPanel
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1147, 773);
        Controls.Add(pnlContent);
        Controls.Add(pnlMenu);
        FormBorderStyle = FormBorderStyle.None;
        Name = "FrmPanel";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "FourAssembly - Production System";
        pnlMenu.ResumeLayout(false);
        ResumeLayout(false);
    }

    #endregion

    private Panel pnlMenu;
    private Button btnStations;
    private Button btnRecipeEditor;
    private Button btnCognexDiagnostics;
    private Panel pnlContent;
}