namespace WinTrains
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private SkiaSharp.Views.Desktop.SKControl _skControl;

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this._skControl = new SkiaSharp.Views.Desktop.SKControl();
            this.SuspendLayout();
            // 
            // _skControl
            // 
            this._skControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this._skControl.PaintSurface += SKControl_PaintSurface;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this._skControl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "Trains";
            this.ResumeLayout(false);

        }

        #endregion
    }
}

