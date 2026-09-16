using Trains.NET.Engine;

namespace Trains.NET.Rendering.UI;

[Order(int.MaxValue)]
public class TooltipScreen : IScreen, ITooltipService
{
    private const int PointerOffsetX = 12;
    private const int PointerOffsetY = 18;
    private const int ScreenMargin = 4;
    private const int PaddingX = 6;
    private const int PaddingY = 4;
    private const int CornerRadius = 3;

    private int _pointerX;
    private int _pointerY;
    private string? _text;

    public event EventHandler? Changed;

    public void PointerMoved(int x, int y)
    {
        _pointerX = x;
        _pointerY = y;
        Hide();
    }

    public void Show(string text)
    {
        _text = text;
        Changed?.Invoke(this, EventArgs.Empty);
    }

    public void Hide()
    {
        if (_text is null)
        {
            return;
        }

        _text = null;
        Changed?.Invoke(this, EventArgs.Empty);
    }

    public void Render(ICanvas canvas, int width, int height)
    {
        if (_text is null)
        {
            return;
        }

        var textHeight = Brushes.Label.TextSize!.Value;
        var tooltipWidth = canvas.MeasureText(_text, Brushes.Label) + (PaddingX * 2);
        var tooltipHeight = textHeight + (PaddingY * 2);

        var left = Math.Min(_pointerX + PointerOffsetX, width - tooltipWidth - ScreenMargin);
        left = Math.Max(ScreenMargin, left);

        var top = _pointerY + PointerOffsetY;
        if (top + tooltipHeight > height - ScreenMargin)
        {
            top = _pointerY - tooltipHeight - PointerOffsetY;
        }

        top = Math.Max(ScreenMargin, top);

        canvas.DrawRoundRect(left, top, tooltipWidth, tooltipHeight, CornerRadius, CornerRadius, Brushes.TooltipBackground);
        canvas.DrawRoundRect(left, top, tooltipWidth, tooltipHeight, CornerRadius, CornerRadius, Brushes.TooltipBorder);
        canvas.DrawText(_text, left + PaddingX, top + PaddingY + textHeight - 2, Brushes.Label);
    }
}
