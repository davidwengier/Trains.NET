namespace Trains.NET.Rendering.UI;

public static class Brushes
{
    public static readonly PaintBrush PanelBorder = new()
    {
        IsAntialias = true,
        Color = Colors.Black,
        Style = PaintStyle.Stroke,
        StrokeWidth = 3
    };
    public static readonly PaintBrush PanelBackground = new()
    {
        IsAntialias = true,
        Style = PaintStyle.Fill,
        Color = Colors.White with { A = 170 }
    };
    public static readonly PaintBrush ButtonBackground = PanelBackground with { Color = Colors.LightGray };
    public static readonly PaintBrush ButtonActiveBackground = PanelBackground with { Color = Colors.LightBlue };
    public static readonly PaintBrush ButtonHoverBackground = PanelBackground with { Color = Colors.LightBlue with { A = 85 } };

    public static readonly PaintBrush Label = new()
    {
        TextSize = 15,
        IsAntialias = true,
        Color = Colors.Black
    };
    public static readonly PaintBrush Disabled = Label with { Color = Colors.LightGray };
    public static readonly PaintBrush Active = Label with { Color = Colors.LightBlue };

    public static readonly PaintBrush Red = PanelBackground with { Color = Colors.LightRed };
}
