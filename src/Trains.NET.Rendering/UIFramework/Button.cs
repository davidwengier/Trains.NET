using System;

namespace Trains.NET.Rendering.UI
{
    public class Button
    {
        private const int ButtonHeight = 20;

        private readonly string? _label;
        private readonly Func<bool>? _isActive;
        private readonly Action? _onClick;
        private bool _isHovered;

        public int Width { get; set; }
        public int Height { get; set; } = ButtonHeight;
        public int PaddingX { get; set; } = 10;
        public bool TransparentBackground { get; set; }
        public PaintBrush LabelBrush { get; set; } = Brushes.Label;

        protected Button()
        {
        }

        public Button(string label, Func<bool> isActive, Action onClick)
        {
            _label = label;
            _isActive = isActive;
            _onClick = onClick;
        }

        public virtual bool HandleMouseAction(int x, int y, PointerAction action)
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

        public virtual int GetMinimumWidth(ICanvas canvas)
        {
            var label = _label ?? throw new InvalidOperationException("No label set, but also not doing custom drawing.");

            return (int)canvas.MeasureText(label, this.LabelBrush) + (this.PaddingX * 2);
        }

        public virtual void Render(ICanvas canvas)
        {
            if (this.Width == 0)
            {
                this.Width = GetMinimumWidth(canvas);
            }

            var label = _label ?? throw new InvalidOperationException("No label set, but also not doing custom drawing.");

            var isActive = _isActive?.Invoke() ?? false;

            PaintBrush brush = isActive ? Brushes.ButtonActiveBackground : Brushes.ButtonBackground;
            if (!this.TransparentBackground || isActive)
            {
                canvas.DrawRect(0, 0, this.Width, this.Height, brush);
            }
            if (_isHovered)
            {
                canvas.DrawRect(0, 0, this.Width, this.Height, Brushes.ButtonHoverBackground);
            }

            var textWidth = canvas.MeasureText(label, this.LabelBrush);

            int textHeight = this.LabelBrush.TextSize ?? throw new NullReferenceException("Must set a text size on the label brush");

            canvas.DrawText(label, (this.Width - textWidth) / 2, textHeight + (float)(this.Height - textHeight) / 2 - 2, this.LabelBrush);
        }
    }
}
