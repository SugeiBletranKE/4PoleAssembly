namespace FourAssembly.Forms;

partial class RecipeEditorForm
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

    private void InitializeComponent()
    {
        lstRecipes = new ListBox();
        btnNew = new Button();
        btnDelete = new Button();
        lblPartNumber = new Label();
        txtPartNumber = new TextBox();
        lblMaterials = new Label();
        dgvMaterials = new DataGridView();
        lblTools = new Label();
        dgvTools = new DataGridView();
        btnSave = new Button();
        pnlLeft = new Panel();
        pnlRight = new Panel();
        ((System.ComponentModel.ISupportInitialize)dgvMaterials).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dgvTools).BeginInit();
        pnlLeft.SuspendLayout();
        pnlRight.SuspendLayout();
        SuspendLayout();
        // 
        // lstRecipes
        // 
        lstRecipes.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        lstRecipes.Font = new Font("Segoe UI", 11F);
        lstRecipes.FormattingEnabled = true;
        lstRecipes.ItemHeight = 20;
        lstRecipes.Location = new Point(10, 9);
        lstRecipes.Margin = new Padding(3, 2, 3, 2);
        lstRecipes.Name = "lstRecipes";
        lstRecipes.Size = new Size(198, 344);
        lstRecipes.TabIndex = 0;
        lstRecipes.SelectedIndexChanged += LstRecipes_SelectedIndexChanged;
        // 
        // btnNew
        // 
        btnNew.Font = new Font("Segoe UI", 10F);
        btnNew.Location = new Point(10, 375);
        btnNew.Margin = new Padding(3, 2, 3, 2);
        btnNew.Name = "btnNew";
        btnNew.Size = new Size(92, 30);
        btnNew.TabIndex = 1;
        btnNew.Text = "New Recipe";
        btnNew.UseVisualStyleBackColor = true;
        btnNew.Click += BtnNew_Click;
        // 
        // btnDelete
        // 
        btnDelete.Font = new Font("Segoe UI", 10F);
        btnDelete.Location = new Point(116, 375);
        btnDelete.Margin = new Padding(3, 2, 3, 2);
        btnDelete.Name = "btnDelete";
        btnDelete.Size = new Size(92, 30);
        btnDelete.TabIndex = 2;
        btnDelete.Text = "Delete";
        btnDelete.UseVisualStyleBackColor = true;
        btnDelete.Click += BtnDelete_Click;
        // 
        // lblPartNumber
        // 
        lblPartNumber.AutoSize = true;
        lblPartNumber.Font = new Font("Segoe UI", 11F);
        lblPartNumber.Location = new Point(10, 9);
        lblPartNumber.Name = "lblPartNumber";
        lblPartNumber.Size = new Size(92, 20);
        lblPartNumber.TabIndex = 0;
        lblPartNumber.Text = "Part Number";
        // 
        // txtPartNumber
        // 
        txtPartNumber.Font = new Font("Segoe UI", 11F);
        txtPartNumber.Location = new Point(10, 30);
        txtPartNumber.Margin = new Padding(3, 2, 3, 2);
        txtPartNumber.Name = "txtPartNumber";
        txtPartNumber.Size = new Size(263, 27);
        txtPartNumber.TabIndex = 1;
        // 
        // lblMaterials
        // 
        lblMaterials.AutoSize = true;
        lblMaterials.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        lblMaterials.Location = new Point(10, 64);
        lblMaterials.Name = "lblMaterials";
        lblMaterials.Size = new Size(74, 20);
        lblMaterials.TabIndex = 2;
        lblMaterials.Text = "Materials";
        // 
        // dgvMaterials
        // 
        dgvMaterials.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        dgvMaterials.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvMaterials.Location = new Point(10, 86);
        dgvMaterials.Margin = new Padding(3, 2, 3, 2);
        dgvMaterials.Name = "dgvMaterials";
        dgvMaterials.RowHeadersWidth = 51;
        dgvMaterials.Size = new Size(460, 105);
        dgvMaterials.TabIndex = 3;
        // 
        // lblTools
        // 
        lblTools.AutoSize = true;
        lblTools.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        lblTools.Location = new Point(10, 202);
        lblTools.Name = "lblTools";
        lblTools.Size = new Size(46, 20);
        lblTools.TabIndex = 4;
        lblTools.Text = "Tools";
        // 
        // dgvTools
        // 
        dgvTools.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        dgvTools.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvTools.Location = new Point(10, 225);
        dgvTools.Margin = new Padding(3, 2, 3, 2);
        dgvTools.Name = "dgvTools";
        dgvTools.RowHeadersWidth = 51;
        dgvTools.Size = new Size(460, 105);
        dgvTools.TabIndex = 5;
        // 
        // btnSave
        // 
        btnSave.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        btnSave.Font = new Font("Segoe UI", 11F);
        btnSave.Location = new Point(405, 345);
        btnSave.Margin = new Padding(3, 2, 3, 2);
        btnSave.Name = "btnSave";
        btnSave.Size = new Size(66, 30);
        btnSave.TabIndex = 6;
        btnSave.Text = "Save";
        btnSave.UseVisualStyleBackColor = true;
        btnSave.Click += BtnSave_Click;
        // 
        // pnlLeft
        // 
        pnlLeft.Controls.Add(btnDelete);
        pnlLeft.Controls.Add(btnNew);
        pnlLeft.Controls.Add(lstRecipes);
        pnlLeft.Dock = DockStyle.Left;
        pnlLeft.Location = new Point(0, 0);
        pnlLeft.Margin = new Padding(3, 2, 3, 2);
        pnlLeft.Name = "pnlLeft";
        pnlLeft.Size = new Size(219, 450);
        pnlLeft.TabIndex = 0;
        // 
        // pnlRight
        // 
        pnlRight.Controls.Add(btnSave);
        pnlRight.Controls.Add(dgvTools);
        pnlRight.Controls.Add(lblTools);
        pnlRight.Controls.Add(dgvMaterials);
        pnlRight.Controls.Add(lblMaterials);
        pnlRight.Controls.Add(txtPartNumber);
        pnlRight.Controls.Add(lblPartNumber);
        pnlRight.Dock = DockStyle.Fill;
        pnlRight.Location = new Point(219, 0);
        pnlRight.Margin = new Padding(3, 2, 3, 2);
        pnlRight.Name = "pnlRight";
        pnlRight.Size = new Size(481, 450);
        pnlRight.TabIndex = 1;
        // 
        // RecipeEditorForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(700, 450);
        Controls.Add(pnlRight);
        Controls.Add(pnlLeft);
        FormBorderStyle = FormBorderStyle.None;
        Margin = new Padding(3, 2, 3, 2);
        Name = "RecipeEditorForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Recipe Editor";
        ((System.ComponentModel.ISupportInitialize)dgvMaterials).EndInit();
        ((System.ComponentModel.ISupportInitialize)dgvTools).EndInit();
        pnlLeft.ResumeLayout(false);
        pnlRight.ResumeLayout(false);
        pnlRight.PerformLayout();
        ResumeLayout(false);
    }

    private ListBox lstRecipes = null!;
    private Button btnNew = null!;
    private Button btnDelete = null!;
    private Label lblPartNumber = null!;
    private TextBox txtPartNumber = null!;
    private Label lblMaterials = null!;
    private DataGridView dgvMaterials = null!;
    private Label lblTools = null!;
    private DataGridView dgvTools = null!;
    private Button btnSave = null!;
    private Panel pnlLeft = null!;
    private Panel pnlRight = null!;
}
