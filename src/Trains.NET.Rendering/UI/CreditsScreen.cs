using System;
using System.Diagnostics;
using Trains.NET.Engine;

namespace Trains.NET.Rendering.UI
{
    [Order(10)]
    public class CreditsScreen : PanelBase
    {
        private const string Label = "https://github.com/davidwengier/Trains.NET";

        private readonly PaintBrush _labelBrush = Brushes.Label;

        protected override PanelPosition Position => PanelPosition.Right;
        protected override bool IsCollapsable => false;
        protected override int TopPadding => 3;
        protected override int BottomPadding => 3;
        protected override int CornerRadius => 5;

        protected override void PreRender(ICanvas canvas)
        {
            // to stick to the bottom
            this.Top = int.MaxValue;

            var textWidth = canvas.MeasureText(Label, _labelBrush);

            int textHeight = _labelBrush.TextSize ?? throw new NullReferenceException("Must set a text size on the label brush");

            this.InnerWidth = Convert.ToInt32(textWidth);
            this.InnerHeight = textHeight;
        }

        protected override void Render(ICanvas canvas)
        {
            int textHeight = _labelBrush.TextSize ?? throw new NullReferenceException("Must set a text size on the label brush");

            canvas.DrawText(Label, 0, (float)textHeight - 2, _labelBrush);
        }

        protected override bool HandlePointerAction(int x, int y, PointerAction action)
        {
            if (action == PointerAction.Click)
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        UseShellExecute = true,
                        Verb = "open",
                        FileName = "https://github.com/davidwengier/trains.net"
                    });
                }
                catch { }
                return true;
            }

            return base.HandlePointerAction(x, y, action);
        }
    }
}
