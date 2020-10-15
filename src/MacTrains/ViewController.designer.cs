// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace MacTrains
{
    [Register("ViewController")]
    partial class ViewController
    {
        [Outlet]
        SkiaSharp.Views.Mac.SKCanvasView canvasView { get; set; }

        void ReleaseDesignerOutlets()
        {
            if (canvasView != null)
            {
                canvasView.Dispose();
                canvasView = null;
            }
        }
    }
}
