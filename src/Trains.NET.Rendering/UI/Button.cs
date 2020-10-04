using System;

namespace Trains.NET.Rendering.UI
{
    public class Button
    {
        private const int TextYPadding = 15;
        private const int ButtonHeight = 20;

        private readonly string _label;
        private readonly Func<bool> _isActive;
        private readonly Action _onClick;

        public int Width { get; set; }
        public int Height { get; set; } = ButtonHeight;

        public Button(string label, Func<bool> isActive, Action onClick)
        {
            _label = label;
            _isActive = isActive;
            _onClick = onClick;
        }

        public void Click()
            => _onClick?.Invoke();

        internal int GetMinimumWidth(ICanvas canvas)
            => (int)canvas.MeasureText(_label, Brushes.Label);

        internal void Render(ICanvas canvas, bool isHovered)
        {
            var isActive = _isActive?.Invoke() ?? false;

            PaintBrush brush = isActive ? Brushes.ButtonActiveBackground : Brushes.ButtonBackground;

            canvas.DrawRect(0, 0, this.Width, this.Height, brush);
            if (isHovered)
            {
                canvas.DrawRect(0, 0, this.Width, this.Height, Brushes.ButtonHoverBackground);
            }

            var textWidth = canvas.MeasureText(_label, Brushes.Label);

            canvas.DrawText(_label, 0 + ((this.Width - textWidth) / 2), TextYPadding, Brushes.Label);
        }
    }
}
