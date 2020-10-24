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

            return (int)canvas.MeasureText(label, Brushes.Label) + (this.PaddingX * 2);
        }

        public virtual void Render(ICanvas canvas)
        {
            if (this.Width == 0)
            {
                this.Width = GetMinimumWidth(canvas);
            }

            var label = _label ?? throw new InvalidOperationException("No label set, but also not doing custom drawing.");

            var isActive = _isActive?.Invoke() ?? false;

            DrawButton(canvas, this.Width, this.Height, label, isActive, _isHovered, Brushes.Label);
        }

        protected static void DrawButton(ICanvas canvas, int width, int height, string label, bool active, bool hovered, PaintBrush labelBrush)
        {
            PaintBrush brush = active ? Brushes.ButtonActiveBackground : Brushes.ButtonBackground;

            canvas.DrawRect(0, 0, width, height, brush);
            if (hovered)
            {
                canvas.DrawRect(0, 0, width, height, Brushes.ButtonHoverBackground);
            }

            var textWidth = canvas.MeasureText(label, labelBrush);

            int textHeight = labelBrush.TextSize ?? throw new NullReferenceException("Must set a text size on the label brush");

            canvas.DrawText(label, (width - textWidth) / 2, textHeight + (float)(height - textHeight) / 2 - 2, labelBrush);
        }
    }
}
