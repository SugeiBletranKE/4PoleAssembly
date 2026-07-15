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
        pnlLeft = new Panel();
        btnDelete = new Button();
        btnNew = new Button();
        lstRecipes = new ListBox();
        scrollPanel = new Panel();
        pnlConnectorGender = new Panel();
        rdbConnectorMale = new RadioButton();
        rdbConnectorFemale = new RadioButton();
        lblConnectorGender = new Label();
        rdb02 = new RadioButton();
        rdb03 = new RadioButton();
        rdb01 = new RadioButton();
        label2 = new Label();
        btnReset = new Button();
        btnSave = new Button();
        btnRemoveTool = new Button();
        lstTools = new ListBox();
        btnAddTool = new Button();
        txtToolBarcode = new TextBox();
        txtToolName = new TextBox();
        lblTools = new Label();
        btnRemoveMaterial = new Button();
        lstMaterials = new ListBox();
        btnAddMaterial = new Button();
        txtMaterialBarcode = new TextBox();
        txtMaterialName = new TextBox();
        lblMaterials = new Label();
        pnlBGInputs = new Panel();
        radBG4 = new RadioButton();
        radBG3 = new RadioButton();
        radBG2 = new RadioButton();
        radBG1 = new RadioButton();
        lblBGs = new Label();
        txtDescription = new TextBox();
        lblDescription = new Label();
        txtPartNumber = new TextBox();
        lblPartNumber = new Label();
        pnlLeft.SuspendLayout();
        scrollPanel.SuspendLayout();
        pnlConnectorGender.SuspendLayout();
        SuspendLayout();
        // 
        // pnlLeft
        // 
        pnlLeft.Controls.Add(btnDelete);
        pnlLeft.Controls.Add(btnNew);
        pnlLeft.Controls.Add(lstRecipes);
        pnlLeft.Dock = DockStyle.Left;
        pnlLeft.Location = new Point(0, 0);
        pnlLeft.Name = "pnlLeft";
        pnlLeft.Size = new Size(261, 629);
        pnlLeft.TabIndex = 0;
        // 
        // btnDelete
        // 
        btnDelete.Font = new Font("Segoe UI", 10F);
        btnDelete.Location = new Point(170, 587);
        btnDelete.Name = "btnDelete";
        btnDelete.Size = new Size(85, 30);
        btnDelete.TabIndex = 2;
        btnDelete.Text = "Delete";
        btnDelete.UseVisualStyleBackColor = true;
        btnDelete.Click += BtnDelete_Click;
        // 
        // btnNew
        // 
        btnNew.Font = new Font("Segoe UI", 10F);
        btnNew.Location = new Point(10, 587);
        btnNew.Name = "btnNew";
        btnNew.Size = new Size(85, 30);
        btnNew.TabIndex = 1;
        btnNew.Text = "New Recipe";
        btnNew.UseVisualStyleBackColor = true;
        btnNew.Click += BtnNew_Click;
        // 
        // lstRecipes
        // 
        lstRecipes.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        lstRecipes.Font = new Font("Segoe UI", 11F);
        lstRecipes.FormattingEnabled = true;
        lstRecipes.ItemHeight = 20;
        lstRecipes.Location = new Point(10, 9);
        lstRecipes.Name = "lstRecipes";
        lstRecipes.Size = new Size(241, 564);
        lstRecipes.TabIndex = 0;
        lstRecipes.SelectedIndexChanged += LstRecipes_SelectedIndexChanged;
        // 
        // scrollPanel
        // 
        scrollPanel.AutoScroll = true;
        scrollPanel.Controls.Add(pnlConnectorGender);
        scrollPanel.Controls.Add(rdb02);
        scrollPanel.Controls.Add(rdb03);
        scrollPanel.Controls.Add(rdb01);
        scrollPanel.Controls.Add(label2);
        scrollPanel.Controls.Add(btnReset);
        scrollPanel.Controls.Add(btnSave);
        scrollPanel.Controls.Add(btnRemoveTool);
        scrollPanel.Controls.Add(lstTools);
        scrollPanel.Controls.Add(btnAddTool);
        scrollPanel.Controls.Add(txtToolBarcode);
        scrollPanel.Controls.Add(txtToolName);
        scrollPanel.Controls.Add(lblTools);
        scrollPanel.Controls.Add(btnRemoveMaterial);
        scrollPanel.Controls.Add(lstMaterials);
        scrollPanel.Controls.Add(btnAddMaterial);
        scrollPanel.Controls.Add(txtMaterialBarcode);
        scrollPanel.Controls.Add(txtMaterialName);
        scrollPanel.Controls.Add(lblMaterials);
        scrollPanel.Controls.Add(pnlBGInputs);
        scrollPanel.Controls.Add(radBG4);
        scrollPanel.Controls.Add(radBG3);
        scrollPanel.Controls.Add(radBG2);
        scrollPanel.Controls.Add(radBG1);
        scrollPanel.Controls.Add(lblBGs);
        scrollPanel.Controls.Add(txtDescription);
        scrollPanel.Controls.Add(lblDescription);
        scrollPanel.Controls.Add(txtPartNumber);
        scrollPanel.Controls.Add(lblPartNumber);
        scrollPanel.Dock = DockStyle.Fill;
        scrollPanel.Location = new Point(261, 0);
        scrollPanel.Name = "scrollPanel";
        scrollPanel.Size = new Size(839, 629);
        scrollPanel.TabIndex = 1;
        // 
        // pnlConnectorGender
        // 
        pnlConnectorGender.Controls.Add(rdbConnectorMale);
        pnlConnectorGender.Controls.Add(rdbConnectorFemale);
        pnlConnectorGender.Controls.Add(lblConnectorGender);
        pnlConnectorGender.Location = new Point(342, 15);
        pnlConnectorGender.Name = "pnlConnectorGender";
        pnlConnectorGender.Size = new Size(260, 55);
        pnlConnectorGender.TabIndex = 27;
        // 
        // rdbConnectorMale
        // 
        rdbConnectorMale.AutoSize = true;
        rdbConnectorMale.Font = new Font("Segoe UI", 10F);
        rdbConnectorMale.Location = new Point(125, 25);
        rdbConnectorMale.Name = "rdbConnectorMale";
        rdbConnectorMale.Size = new Size(57, 23);
        rdbConnectorMale.TabIndex = 2;
        rdbConnectorMale.Text = "Male";
        rdbConnectorMale.UseVisualStyleBackColor = true;
        // 
        // rdbConnectorFemale
        // 
        rdbConnectorFemale.AutoSize = true;
        rdbConnectorFemale.Checked = true;
        rdbConnectorFemale.Font = new Font("Segoe UI", 10F);
        rdbConnectorFemale.Location = new Point(15, 25);
        rdbConnectorFemale.Name = "rdbConnectorFemale";
        rdbConnectorFemale.Size = new Size(70, 23);
        rdbConnectorFemale.TabIndex = 1;
        rdbConnectorFemale.TabStop = true;
        rdbConnectorFemale.Text = "Female";
        rdbConnectorFemale.UseVisualStyleBackColor = true;
        // 
        // lblConnectorGender
        // 
        lblConnectorGender.AutoSize = true;
        lblConnectorGender.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        lblConnectorGender.Location = new Point(0, 0);
        lblConnectorGender.Name = "lblConnectorGender";
        lblConnectorGender.Size = new Size(116, 20);
        lblConnectorGender.TabIndex = 0;
        lblConnectorGender.Text = "Connector type";
        // 
        // rdb02
        // 
        rdb02.AutoSize = true;
        rdb02.Font = new Font("Segoe UI", 10F);
        rdb02.Location = new Point(115, 257);
        rdb02.Name = "rdb02";
        rdb02.Size = new Size(90, 23);
        rdb02.TabIndex = 24;
        rdb02.Text = "Station 02";
        rdb02.UseVisualStyleBackColor = true;
        // 
        // rdb03
        // 
        rdb03.AutoSize = true;
        rdb03.Font = new Font("Segoe UI", 10F);
        rdb03.Location = new Point(211, 257);
        rdb03.Name = "rdb03";
        rdb03.Size = new Size(90, 23);
        rdb03.TabIndex = 23;
        rdb03.Text = "Station 03";
        rdb03.UseVisualStyleBackColor = true;
        // 
        // rdb01
        // 
        rdb01.AutoSize = true;
        rdb01.Font = new Font("Segoe UI", 10F);
        rdb01.Location = new Point(15, 257);
        rdb01.Name = "rdb01";
        rdb01.Size = new Size(90, 23);
        rdb01.TabIndex = 22;
        rdb01.Text = "Station 01";
        rdb01.UseVisualStyleBackColor = true;
        // 
        // label2
        // 
        label2.AutoSize = true;
        label2.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        label2.Location = new Point(15, 232);
        label2.Name = "label2";
        label2.Size = new Size(164, 20);
        label2.TabIndex = 21;
        label2.Text = "Liberación de material";
        // 
        // btnReset
        // 
        btnReset.Font = new Font("Segoe UI", 11F);
        btnReset.Location = new Point(115, 582);
        btnReset.Name = "btnReset";
        btnReset.Size = new Size(100, 35);
        btnReset.TabIndex = 20;
        btnReset.Text = "Reset";
        btnReset.UseVisualStyleBackColor = true;
        btnReset.Click += BtnReset_Click;
        // 
        // btnSave
        // 
        btnSave.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        btnSave.Location = new Point(5, 582);
        btnSave.Name = "btnSave";
        btnSave.Size = new Size(100, 35);
        btnSave.TabIndex = 19;
        btnSave.Text = "Save";
        btnSave.UseVisualStyleBackColor = true;
        btnSave.Click += BtnSave_Click;
        // 
        // btnRemoveTool
        // 
        btnRemoveTool.Font = new Font("Segoe UI", 10F);
        btnRemoveTool.Location = new Point(15, 524);
        btnRemoveTool.Name = "btnRemoveTool";
        btnRemoveTool.Size = new Size(100, 26);
        btnRemoveTool.TabIndex = 18;
        btnRemoveTool.Text = "Remove";
        btnRemoveTool.UseVisualStyleBackColor = true;
        btnRemoveTool.Click += BtnRemoveTool_Click;
        // 
        // lstTools
        // 
        lstTools.Font = new Font("Segoe UI", 10F);
        lstTools.FormattingEnabled = true;
        lstTools.ItemHeight = 17;
        lstTools.Location = new Point(15, 480);
        lstTools.Name = "lstTools";
        lstTools.Size = new Size(420, 38);
        lstTools.TabIndex = 17;
        // 
        // btnAddTool
        // 
        btnAddTool.Font = new Font("Segoe UI", 10F);
        btnAddTool.Location = new Point(375, 448);
        btnAddTool.Name = "btnAddTool";
        btnAddTool.Size = new Size(60, 26);
        btnAddTool.TabIndex = 16;
        btnAddTool.Text = "Add";
        btnAddTool.UseVisualStyleBackColor = true;
        btnAddTool.Click += BtnAddTool_Click;
        // 
        // txtToolBarcode
        // 
        txtToolBarcode.Font = new Font("Segoe UI", 10F);
        txtToolBarcode.Location = new Point(225, 448);
        txtToolBarcode.Name = "txtToolBarcode";
        txtToolBarcode.PlaceholderText = "Barcode";
        txtToolBarcode.Size = new Size(140, 25);
        txtToolBarcode.TabIndex = 15;
        // 
        // txtToolName
        // 
        txtToolName.Font = new Font("Segoe UI", 10F);
        txtToolName.Location = new Point(15, 448);
        txtToolName.Name = "txtToolName";
        txtToolName.PlaceholderText = "Tool name";
        txtToolName.Size = new Size(200, 25);
        txtToolName.TabIndex = 14;
        // 
        // lblTools
        // 
        lblTools.AutoSize = true;
        lblTools.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        lblTools.Location = new Point(15, 425);
        lblTools.Name = "lblTools";
        lblTools.Size = new Size(46, 20);
        lblTools.TabIndex = 13;
        lblTools.Text = "Tools";
        // 
        // btnRemoveMaterial
        // 
        btnRemoveMaterial.Font = new Font("Segoe UI", 10F);
        btnRemoveMaterial.Location = new Point(15, 396);
        btnRemoveMaterial.Name = "btnRemoveMaterial";
        btnRemoveMaterial.Size = new Size(100, 26);
        btnRemoveMaterial.TabIndex = 12;
        btnRemoveMaterial.Text = "Remove";
        btnRemoveMaterial.UseVisualStyleBackColor = true;
        btnRemoveMaterial.Click += BtnRemoveMaterial_Click;
        // 
        // lstMaterials
        // 
        lstMaterials.Font = new Font("Segoe UI", 10F);
        lstMaterials.FormattingEnabled = true;
        lstMaterials.ItemHeight = 17;
        lstMaterials.Location = new Point(15, 352);
        lstMaterials.Name = "lstMaterials";
        lstMaterials.Size = new Size(420, 38);
        lstMaterials.TabIndex = 11;
        // 
        // btnAddMaterial
        // 
        btnAddMaterial.Font = new Font("Segoe UI", 10F);
        btnAddMaterial.Location = new Point(375, 320);
        btnAddMaterial.Name = "btnAddMaterial";
        btnAddMaterial.Size = new Size(60, 26);
        btnAddMaterial.TabIndex = 10;
        btnAddMaterial.Text = "Add";
        btnAddMaterial.UseVisualStyleBackColor = true;
        btnAddMaterial.Click += BtnAddMaterial_Click;
        // 
        // txtMaterialBarcode
        // 
        txtMaterialBarcode.Font = new Font("Segoe UI", 10F);
        txtMaterialBarcode.Location = new Point(225, 320);
        txtMaterialBarcode.Name = "txtMaterialBarcode";
        txtMaterialBarcode.PlaceholderText = "Barcode";
        txtMaterialBarcode.Size = new Size(140, 25);
        txtMaterialBarcode.TabIndex = 9;
        // 
        // txtMaterialName
        // 
        txtMaterialName.Font = new Font("Segoe UI", 10F);
        txtMaterialName.Location = new Point(15, 320);
        txtMaterialName.Name = "txtMaterialName";
        txtMaterialName.PlaceholderText = "Material name";
        txtMaterialName.Size = new Size(200, 25);
        txtMaterialName.TabIndex = 8;
        // 
        // lblMaterials
        // 
        lblMaterials.AutoSize = true;
        lblMaterials.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        lblMaterials.Location = new Point(15, 297);
        lblMaterials.Name = "lblMaterials";
        lblMaterials.Size = new Size(74, 20);
        lblMaterials.TabIndex = 7;
        lblMaterials.Text = "Materials";
        // 
        // pnlBGInputs
        // 
        pnlBGInputs.Location = new Point(15, 190);
        pnlBGInputs.Name = "pnlBGInputs";
        pnlBGInputs.Size = new Size(500, 30);
        pnlBGInputs.TabIndex = 9;
        // 
        // radBG4
        // 
        radBG4.AutoSize = true;
        radBG4.Font = new Font("Segoe UI", 10F);
        radBG4.Location = new Point(330, 160);
        radBG4.Name = "radBG4";
        radBG4.Size = new Size(63, 23);
        radBG4.TabIndex = 8;
        radBG4.Text = "4 BGs";
        radBG4.UseVisualStyleBackColor = true;
        radBG4.CheckedChanged += RadBG_CheckedChanged;
        // 
        // radBG3
        // 
        radBG3.AutoSize = true;
        radBG3.Font = new Font("Segoe UI", 10F);
        radBG3.Location = new Point(225, 160);
        radBG3.Name = "radBG3";
        radBG3.Size = new Size(63, 23);
        radBG3.TabIndex = 7;
        radBG3.Text = "3 BGs";
        radBG3.UseVisualStyleBackColor = true;
        radBG3.CheckedChanged += RadBG_CheckedChanged;
        // 
        // radBG2
        // 
        radBG2.AutoSize = true;
        radBG2.Font = new Font("Segoe UI", 10F);
        radBG2.Location = new Point(120, 160);
        radBG2.Name = "radBG2";
        radBG2.Size = new Size(63, 23);
        radBG2.TabIndex = 6;
        radBG2.Text = "2 BGs";
        radBG2.UseVisualStyleBackColor = true;
        radBG2.CheckedChanged += RadBG_CheckedChanged;
        // 
        // radBG1
        // 
        radBG1.AutoSize = true;
        radBG1.Font = new Font("Segoe UI", 10F);
        radBG1.Location = new Point(15, 160);
        radBG1.Name = "radBG1";
        radBG1.Size = new Size(57, 23);
        radBG1.TabIndex = 5;
        radBG1.Text = "1 BG";
        radBG1.UseVisualStyleBackColor = true;
        radBG1.CheckedChanged += RadBG_CheckedChanged;
        // 
        // lblBGs
        // 
        lblBGs.AutoSize = true;
        lblBGs.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        lblBGs.Location = new Point(15, 135);
        lblBGs.Name = "lblBGs";
        lblBGs.Size = new Size(118, 20);
        lblBGs.TabIndex = 4;
        lblBGs.Text = "Number of BGs";
        // 
        // txtDescription
        // 
        txtDescription.Font = new Font("Segoe UI", 11F);
        txtDescription.Location = new Point(15, 98);
        txtDescription.Name = "txtDescription";
        txtDescription.Size = new Size(300, 27);
        txtDescription.TabIndex = 3;
        // 
        // lblDescription
        // 
        lblDescription.AutoSize = true;
        lblDescription.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        lblDescription.Location = new Point(15, 75);
        lblDescription.Name = "lblDescription";
        lblDescription.Size = new Size(89, 20);
        lblDescription.TabIndex = 2;
        lblDescription.Text = "Description";
        // 
        // txtPartNumber
        // 
        txtPartNumber.Font = new Font("Segoe UI", 11F);
        txtPartNumber.Location = new Point(15, 38);
        txtPartNumber.Name = "txtPartNumber";
        txtPartNumber.Size = new Size(300, 27);
        txtPartNumber.TabIndex = 1;
        // 
        // lblPartNumber
        // 
        lblPartNumber.AutoSize = true;
        lblPartNumber.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        lblPartNumber.Location = new Point(15, 15);
        lblPartNumber.Name = "lblPartNumber";
        lblPartNumber.Size = new Size(100, 20);
        lblPartNumber.TabIndex = 0;
        lblPartNumber.Text = "Part Number";
        // 
        // RecipeEditorForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1100, 629);
        Controls.Add(scrollPanel);
        Controls.Add(pnlLeft);
        FormBorderStyle = FormBorderStyle.FixedToolWindow;
        Name = "RecipeEditorForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Recipe Editor";
        pnlLeft.ResumeLayout(false);
        scrollPanel.ResumeLayout(false);
        scrollPanel.PerformLayout();
        pnlConnectorGender.ResumeLayout(false);
        pnlConnectorGender.PerformLayout();
        ResumeLayout(false);
    }

    private Panel pnlLeft = null!;
    private ListBox lstRecipes = null!;
    private Button btnNew = null!;
    private Button btnDelete = null!;

    private Panel scrollPanel = null!;

    private Label lblPartNumber = null!;
    private TextBox txtPartNumber = null!;

    private Label lblDescription = null!;
    private TextBox txtDescription = null!;

    private Label lblBGs = null!;
    private RadioButton radBG1 = null!;
    private RadioButton radBG2 = null!;
    private RadioButton radBG3 = null!;
    private RadioButton radBG4 = null!;

    internal Panel pnlBGInputs = null!;

    private Label lblMaterials = null!;
    private TextBox txtMaterialName = null!;
    private TextBox txtMaterialBarcode = null!;
    private Button btnAddMaterial = null!;
    private ListBox lstMaterials = null!;
    private Button btnRemoveMaterial = null!;

    private Label lblTools = null!;
    private TextBox txtToolName = null!;
    private TextBox txtToolBarcode = null!;
    private Button btnAddTool = null!;
    private ListBox lstTools = null!;
    private Button btnRemoveTool = null!;

    private Button btnSave = null!;
    private Button btnReset = null!;
    private Label label1;
    private RadioButton rdb02;
    private RadioButton rdb03;
    private RadioButton rdb01;
    private Label label2;
    private Panel pnlConnectorGender;
    private RadioButton rdbConnectorMale;
    private RadioButton rdbConnectorFemale;
    private Label lblConnectorGender;
}
