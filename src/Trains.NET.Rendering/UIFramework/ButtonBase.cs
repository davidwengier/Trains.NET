using System;

namespace Trains.NET.Rendering.UI;

public abstract class ButtonBase
{
    private const int ButtonHeight = 20;

    private readonly Func<bool>? _isActive;
    private readonly Action? _onClick;
    private bool _isHovered;

    public int Width { get; set; }
    public int Height { get; set; } = ButtonHeight;
    public int PaddingX { get; set; } = 10;
    public bool TransparentBackground { get; set; }

    protected ButtonBase()
    {
    }

    protected ButtonBase(Func<bool> isActive, Action onClick)
    {
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

    public virtual void Render(ICanvas canvas)
    {
        if (this.Width == 0)
        {
            this.Width = GetMinimumWidth(canvas);
        }

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

        RenderButtonLabel(canvas);
    }

    public abstract int GetMinimumWidth(ICanvas canvas);
    protected abstract void RenderButtonLabel(ICanvas canvas);
}
