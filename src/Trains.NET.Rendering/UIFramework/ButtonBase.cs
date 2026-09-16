namespace Trains.NET.Rendering.UI;

public abstract class ButtonBase
{
    private const int ButtonHeight = 20;

    private readonly Func<bool>? _isActive;
    private readonly Action? _onClick;
    private readonly ITooltipService? _tooltipService;
    private readonly string? _tooltip;
    private bool _isHovered;

    public int Width { get; set; }
    public int Height { get; set; } = ButtonHeight;
    public int PaddingX { get; set; } = 10;
    public bool TransparentBackground { get; set; }

    protected ButtonBase()
    {
    }

    protected ButtonBase(
        Func<bool> isActive,
        Action onClick,
        ITooltipService? tooltipService = null,
        string? tooltip = null)
    {
        if ((tooltipService is null) != (tooltip is null))
        {
            throw new ArgumentException("Tooltip text and service must be provided together");
        }

        _isActive = isActive;
        _onClick = onClick;
        _tooltipService = tooltipService;
        _tooltip = tooltip;
    }

    public virtual bool HandleMouseAction(int x, int y, PointerAction action)
    {
        if (x is >= 0 && x <= Width && y >= 0 && y <= Height)
        {
            if (action == PointerAction.Click)
            {
                _onClick?.Invoke();
            }
            else
            {
                _isHovered = true;
                if (_tooltip is not null)
                {
                    _tooltipService!.Show(_tooltip);
                }
            }

            return true;
        }

        _isHovered = false;
        return false;
    }

    public virtual void Render(ICanvas canvas)
    {
        if (Width == 0)
        {
            Width = GetMinimumWidth(canvas);
        }

        var isActive = _isActive?.Invoke() ?? false;

        var brush = isActive ? Brushes.ButtonActiveBackground : Brushes.ButtonBackground;
        if (!TransparentBackground || isActive)
        {
            canvas.DrawRect(0, 0, Width, Height, brush);
        }

        if (_isHovered)
        {
            canvas.DrawRect(0, 0, Width, Height, Brushes.ButtonHoverBackground);
        }

        RenderButtonLabel(canvas);
    }

    public abstract int GetMinimumWidth(ICanvas canvas);
    protected abstract void RenderButtonLabel(ICanvas canvas);
}
