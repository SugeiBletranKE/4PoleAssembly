#nullable disable

namespace FourAssembly.Forms;

partial class StationCountersForm
{
    private System.ComponentModel.IContainer components = null;
    private Label lblTitle;
    private TableLayoutPanel countersTable;
    private Label lblStation1;
    private Label lblStation2;
    private Label lblStation3;
    private Label lblCounter1;
    private Label lblCounter2;
    private Label lblCounter3;
    private Button btnResetTotal1;
    private Button btnResetTotal2;
    private Button btnResetTotal3;
    private Button btnResetBg1;
    private Button btnResetBg2;
    private Button btnResetBg3;
    private Button btnResetHour1;
    private Button btnResetHour2;
    private Button btnResetHour3;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            components?.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        lblTitle = new Label();
        countersTable = new TableLayoutPanel();
        lblStation1 = new Label();
        lblCounter1 = new Label();
        btnResetTotal1 = new Button();
        btnResetBg1 = new Button();
        btnResetHour1 = new Button();
        lblStation2 = new Label();
        lblCounter2 = new Label();
        btnResetTotal2 = new Button();
        btnResetBg2 = new Button();
        btnResetHour2 = new Button();
        lblStation3 = new Label();
        lblCounter3 = new Label();
        btnResetTotal3 = new Button();
        btnResetBg3 = new Button();
        btnResetHour3 = new Button();
        countersTable.SuspendLayout();
        SuspendLayout();
        // 
        // lblTitle
        // 
        lblTitle.Dock = DockStyle.Top;
        lblTitle.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
        lblTitle.ForeColor = Color.FromArgb(20, 55, 100);
        lblTitle.Location = new Point(0, 0);
        lblTitle.Name = "lblTitle";
        lblTitle.Size = new Size(1050, 70);
        lblTitle.TabIndex = 0;
        lblTitle.Text = "CONTADORES DE PIEZAS OK";
        lblTitle.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // countersTable
        // 
        countersTable.BackColor = Color.White;
        countersTable.ColumnCount = 5;
        countersTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160F));
        countersTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        countersTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 175F));
        countersTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 175F));
        countersTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 175F));
        countersTable.Controls.Add(lblStation1, 0, 0);
        countersTable.Controls.Add(lblCounter1, 1, 0);
        countersTable.Controls.Add(btnResetTotal1, 2, 0);
        countersTable.Controls.Add(btnResetBg1, 3, 0);
        countersTable.Controls.Add(btnResetHour1, 4, 0);
        countersTable.Controls.Add(lblStation2, 0, 1);
        countersTable.Controls.Add(lblCounter2, 1, 1);
        countersTable.Controls.Add(btnResetTotal2, 2, 1);
        countersTable.Controls.Add(btnResetBg2, 3, 1);
        countersTable.Controls.Add(btnResetHour2, 4, 1);
        countersTable.Controls.Add(lblStation3, 0, 2);
        countersTable.Controls.Add(lblCounter3, 1, 2);
        countersTable.Controls.Add(btnResetTotal3, 2, 2);
        countersTable.Controls.Add(btnResetBg3, 3, 2);
        countersTable.Controls.Add(btnResetHour3, 4, 2);
        countersTable.Dock = DockStyle.Fill;
        countersTable.Location = new Point(0, 70);
        countersTable.Name = "countersTable";
        countersTable.Padding = new Padding(15);
        countersTable.RowCount = 3;
        countersTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 110F));
        countersTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 110F));
        countersTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 110F));
        countersTable.Size = new Size(1050, 350);
        countersTable.TabIndex = 1;
        // 
        // labels
        // 
        lblStation1.Dock = DockStyle.Fill;
        lblStation1.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
        lblStation1.Name = "lblStation1";
        lblStation1.Text = "Estación 1";
        lblStation1.TextAlign = ContentAlignment.MiddleLeft;
        lblCounter1.Dock = DockStyle.Fill;
        lblCounter1.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        lblCounter1.ForeColor = Color.DarkGreen;
        lblCounter1.Name = "lblCounter1";
        lblCounter1.Text = "Total estación: 0\r\nBG --: 0\r\n00:00–01:00: 0";
        lblCounter1.TextAlign = ContentAlignment.MiddleLeft;
        lblStation2.Dock = DockStyle.Fill;
        lblStation2.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
        lblStation2.Name = "lblStation2";
        lblStation2.Text = "Estación 2";
        lblStation2.TextAlign = ContentAlignment.MiddleLeft;
        lblCounter2.Dock = DockStyle.Fill;
        lblCounter2.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        lblCounter2.ForeColor = Color.DarkGreen;
        lblCounter2.Name = "lblCounter2";
        lblCounter2.Text = "Total estación: 0\r\nBG --: 0\r\n00:00–01:00: 0";
        lblCounter2.TextAlign = ContentAlignment.MiddleLeft;
        lblStation3.Dock = DockStyle.Fill;
        lblStation3.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
        lblStation3.Name = "lblStation3";
        lblStation3.Text = "Estación 3";
        lblStation3.TextAlign = ContentAlignment.MiddleLeft;
        lblCounter3.Dock = DockStyle.Fill;
        lblCounter3.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        lblCounter3.ForeColor = Color.DarkGreen;
        lblCounter3.Name = "lblCounter3";
        lblCounter3.Text = "Total estación: 0\r\nBG --: 0\r\n00:00–01:00: 0";
        lblCounter3.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // reset buttons
        // 
        btnResetTotal1.BackColor = Color.MistyRose;
        btnResetTotal1.Dock = DockStyle.Fill;
        btnResetTotal1.FlatStyle = FlatStyle.Flat;
        btnResetTotal1.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        btnResetTotal1.Margin = new Padding(7, 22, 7, 22);
        btnResetTotal1.Name = "btnResetTotal1";
        btnResetTotal1.Text = "Reiniciar total";
        btnResetTotal1.UseVisualStyleBackColor = false;
        btnResetTotal1.Click += ResetButton_Click;
        btnResetBg1.BackColor = Color.LemonChiffon;
        btnResetBg1.Dock = DockStyle.Fill;
        btnResetBg1.FlatStyle = FlatStyle.Flat;
        btnResetBg1.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        btnResetBg1.Margin = new Padding(7, 22, 7, 22);
        btnResetBg1.Name = "btnResetBg1";
        btnResetBg1.Text = "Reiniciar BG";
        btnResetBg1.UseVisualStyleBackColor = false;
        btnResetBg1.Click += ResetButton_Click;
        btnResetHour1.BackColor = Color.AliceBlue;
        btnResetHour1.Dock = DockStyle.Fill;
        btnResetHour1.FlatStyle = FlatStyle.Flat;
        btnResetHour1.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        btnResetHour1.Margin = new Padding(7, 22, 7, 22);
        btnResetHour1.Name = "btnResetHour1";
        btnResetHour1.Text = "Reiniciar hora";
        btnResetHour1.UseVisualStyleBackColor = false;
        btnResetHour1.Click += ResetButton_Click;
        btnResetTotal2.BackColor = Color.MistyRose;
        btnResetTotal2.Dock = DockStyle.Fill;
        btnResetTotal2.FlatStyle = FlatStyle.Flat;
        btnResetTotal2.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        btnResetTotal2.Margin = new Padding(7, 22, 7, 22);
        btnResetTotal2.Name = "btnResetTotal2";
        btnResetTotal2.Text = "Reiniciar total";
        btnResetTotal2.UseVisualStyleBackColor = false;
        btnResetTotal2.Click += ResetButton_Click;
        btnResetBg2.BackColor = Color.LemonChiffon;
        btnResetBg2.Dock = DockStyle.Fill;
        btnResetBg2.FlatStyle = FlatStyle.Flat;
        btnResetBg2.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        btnResetBg2.Margin = new Padding(7, 22, 7, 22);
        btnResetBg2.Name = "btnResetBg2";
        btnResetBg2.Text = "Reiniciar BG";
        btnResetBg2.UseVisualStyleBackColor = false;
        btnResetBg2.Click += ResetButton_Click;
        btnResetHour2.BackColor = Color.AliceBlue;
        btnResetHour2.Dock = DockStyle.Fill;
        btnResetHour2.FlatStyle = FlatStyle.Flat;
        btnResetHour2.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        btnResetHour2.Margin = new Padding(7, 22, 7, 22);
        btnResetHour2.Name = "btnResetHour2";
        btnResetHour2.Text = "Reiniciar hora";
        btnResetHour2.UseVisualStyleBackColor = false;
        btnResetHour2.Click += ResetButton_Click;
        btnResetTotal3.BackColor = Color.MistyRose;
        btnResetTotal3.Dock = DockStyle.Fill;
        btnResetTotal3.FlatStyle = FlatStyle.Flat;
        btnResetTotal3.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        btnResetTotal3.Margin = new Padding(7, 22, 7, 22);
        btnResetTotal3.Name = "btnResetTotal3";
        btnResetTotal3.Text = "Reiniciar total";
        btnResetTotal3.UseVisualStyleBackColor = false;
        btnResetTotal3.Click += ResetButton_Click;
        btnResetBg3.BackColor = Color.LemonChiffon;
        btnResetBg3.Dock = DockStyle.Fill;
        btnResetBg3.FlatStyle = FlatStyle.Flat;
        btnResetBg3.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        btnResetBg3.Margin = new Padding(7, 22, 7, 22);
        btnResetBg3.Name = "btnResetBg3";
        btnResetBg3.Text = "Reiniciar BG";
        btnResetBg3.UseVisualStyleBackColor = false;
        btnResetBg3.Click += ResetButton_Click;
        btnResetHour3.BackColor = Color.AliceBlue;
        btnResetHour3.Dock = DockStyle.Fill;
        btnResetHour3.FlatStyle = FlatStyle.Flat;
        btnResetHour3.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        btnResetHour3.Margin = new Padding(7, 22, 7, 22);
        btnResetHour3.Name = "btnResetHour3";
        btnResetHour3.Text = "Reiniciar hora";
        btnResetHour3.UseVisualStyleBackColor = false;
        btnResetHour3.Click += ResetButton_Click;
        // 
        // StationCountersForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.WhiteSmoke;
        ClientSize = new Size(1050, 420);
        Controls.Add(countersTable);
        Controls.Add(lblTitle);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "StationCountersForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Contadores de producción";
        countersTable.ResumeLayout(false);
        ResumeLayout(false);
    }
}
