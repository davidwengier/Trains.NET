namespace Trains.NET.Rendering.UI;

public class TextButton(string label, Func<bool> isActive, Action onClick) : ButtonBase(isActive, onClick)
{
    private readonly string _label = label;
    public PaintBrush LabelBrush { get; set; } = Brushes.Label;

    public override int GetMinimumWidth(ICanvas canvas)
    {
        return (int)canvas.MeasureText(_label, this.LabelBrush) + (this.PaddingX * 2);
    }

    protected override void RenderButtonLabel(ICanvas canvas)
    {
        var textWidth = canvas.MeasureText(_label, this.LabelBrush);

        int textHeight = this.LabelBrush.TextSize ?? throw new NullReferenceException("Must set a text size on the label brush");

        canvas.DrawText(_label, (this.Width - textWidth) / 2, textHeight + (float)(this.Height - textHeight) / 2 - 2, this.LabelBrush);
    }
}
