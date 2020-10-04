using System;

namespace Trains.NET.Rendering.UI
{
    public class Button
    {
        private const int ButtonHeight = 20;

        private readonly string _label;
        private readonly Func<bool> _isActive;
        private readonly Action _onClick;
        private bool _isHovered;

        public int Width { get; set; }
        public int Height { get; set; } = ButtonHeight;
        public int PaddingX { get; set; } = 10;

        public Button(string label, Func<bool> isActive, Action onClick)
        {
            _label = label;
            _isActive = isActive;
            _onClick = onClick;
        }

        public bool HandleMouseAction(int x, int y, PointerAction action)
        {
            if (x is >= 0 && x <= this.Width && y >= 0 && y <= this.Height)
            {
                if (action == PointerAction.Click)
                {
                    _onClick?.Invoke();
                }
                else
                {
                    _isHovered = true;
                }
                return true;
            }
            _isHovered = false;
            return false;
        }

        public int GetMinimumWidth(ICanvas canvas)
            => (int)canvas.MeasureText(_label, Brushes.Label) + (this.PaddingX * 2);

        public void Render(ICanvas canvas)
        {
            if (this.Width == 0)
            {
                this.Width = GetMinimumWidth(canvas);
            }

            var isActive = _isActive?.Invoke() ?? false;

            PaintBrush brush = isActive ? Brushes.ButtonActiveBackground : Brushes.ButtonBackground;

            canvas.DrawRect(0, 0, this.Width, this.Height, brush);
            if (_isHovered)
            {
                canvas.DrawRect(0, 0, this.Width, this.Height, Brushes.ButtonHoverBackground);
            }

            var textWidth = canvas.MeasureText(_label, Brushes.Label);

            canvas.DrawText(_label, 0 + ((this.Width - textWidth) / 2), (float)Brushes.Label.TextSize!, Brushes.Label);
        }
    }
}
