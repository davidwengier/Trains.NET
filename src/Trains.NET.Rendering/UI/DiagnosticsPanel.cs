using Trains.NET.Engine;
using Trains.NET.Instrumentation;
using Trains.NET.Rendering.UI;

namespace Trains.NET.Rendering;

[Order(1000)]
public class DiagnosticsPanel : PanelBase, ITogglable
{
    private const int LineGap = 3;

    public bool Enabled
    {
        get { return Visible; }
        set
        {
            Visible = value;
            OnChanged();
        }
    }

    public string Name => "Diagnostics";

    protected override PanelPosition Position => PanelPosition.Floating;
    protected override int BottomPadding => 2;
    protected override int TopPadding => 2;
    protected override int CornerRadius => 2;

    protected override PaintBrush PanelBorderBrush { get; } = Brushes.PanelBorder with { StrokeWidth = 1 };

    public DiagnosticsPanel()
    {
        Left = 10;
        Visible = false;
    }

    protected override void PreRender(ICanvas canvas)
    {
        Top = int.MaxValue;
    }

    protected override void Render(ICanvas canvas)
    {
        var lineHeight = Brushes.Label.TextSize.GetValueOrDefault();

        float maxWidth = 0;
        var strings = new List<string>();
        foreach ((var name, var stat) in InstrumentationBag.Stats.OrderBy(i => i.Name))
        {
            if (stat.ShouldShow())
            {
                var line = name + ": " + stat.GetDescription();
                strings.Add(line);
                maxWidth = Math.Max(maxWidth, canvas.MeasureText(line, Brushes.Label));
            }
        }

        InnerWidth = (int)maxWidth;
        InnerHeight = strings.Count * (lineHeight + LineGap);

        foreach (var line in strings)
        {
            canvas.DrawText(line, 0, lineHeight, Brushes.Label);
            canvas.Translate(0, LineGap + lineHeight);
        }
    }
}
