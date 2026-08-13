namespace Trains.NET.Rendering.UI;

public class TextButton(string label, Func<bool> isActive, Action onClick) : ButtonBase(isActive, onClick)
{
    private readonly string _label = label;
    public PaintBrush LabelBrush { get; set; } = Brushes.Label;

    public override int GetMinimumWidth(ICanvas canvas)
    {
        return (int)canvas.MeasureText(_label, LabelBrush) + (PaddingX * 2);
    }

    protected override void RenderButtonLabel(ICanvas canvas)
    {
        var textWidth = canvas.MeasureText(_label, LabelBrush);

        var textHeight = LabelBrush.TextSize ?? throw new NullReferenceException("Must set a text size on the label brush");

        canvas.DrawText(_label, (Width - textWidth) / 2, textHeight + (float)(Height - textHeight) / 2 - 2, LabelBrush);
    }
}
