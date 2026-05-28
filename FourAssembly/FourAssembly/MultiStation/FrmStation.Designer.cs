namespace FourAssembly.MultiStation
{
    partial class FrmStation
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
            SuspendLayout();
            // 
            // FrmStation
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(900, 600);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FrmStation";
            Text = "Station";
            ResumeLayout(false);
        }
    }
}
