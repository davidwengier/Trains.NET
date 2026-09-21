using Trains.NET.Engine;

namespace Trains.NET.Rendering;

public class DepotRenderer : IStaticEntityRenderer<Depot>
{
    private static readonly PaintBrush BuildingBrush = new()
    {
        Color = Colors.DarkRed,
        IsAntialias = true,
        Style = PaintStyle.Fill
    };

    private static readonly PaintBrush RoofBrush = new()
    {
        Color = Colors.LightRed,
        IsAntialias = true,
        Style = PaintStyle.Fill
    };

    private static readonly PaintBrush DoorBrush = new()
    {
        Color = Colors.VeryDarkGray,
        Style = PaintStyle.Fill
    };

    private static readonly PaintBrush RailBrush = new()
    {
        Color = Colors.Gray,
        IsAntialias = true,
        Style = PaintStyle.Stroke,
        StrokeWidth = 4
    };

    public void Render(ICanvas canvas, Depot depot)
    {
        canvas.RotateDegrees(depot.Rotation, 50, 50);

        canvas.DrawRoundRect(8, 12, 76, 76, 5, 5, BuildingBrush);
        canvas.DrawRoundRect(4, 8, 84, 18, 5, 5, RoofBrush);
        canvas.DrawRect(62, 32, 30, 44, DoorBrush);

        canvas.DrawLine(84, 40, 100, 40, RailBrush);
        canvas.DrawLine(84, 68, 100, 68, RailBrush);
    }
}
