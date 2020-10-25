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
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Text = "Form1";
            ///
            /// _skControl
            ///
            this._skControl = new SkiaSharp.Views.Desktop.SKControl();
            this._skControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this._skControl.PaintSurface += SKControl_PaintSurface;
            this.Controls.Add(_skControl);
        }

        #endregion
    }
}

