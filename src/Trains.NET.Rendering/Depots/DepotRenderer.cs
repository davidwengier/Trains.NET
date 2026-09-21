using Trains.NET.Engine;

namespace Trains.NET.Rendering;

public class DepotRenderer(
    SingleTrackRenderer trackRenderer,
    ITrackParameters trackParameters) : IStaticEntityRenderer<Depot>
{
    private const float BuildingRight = 84;
    private const float TrackStart = 58;

    private readonly SingleTrackRenderer _trackRenderer = trackRenderer;
    private readonly ITrackParameters _trackParameters = trackParameters;

    private static readonly PaintBrush BuildingBrush = new()
    {
        Color = Colors.DarkRed,
        IsAntialias = true,
        Style = PaintStyle.Fill
    };

    private static readonly PaintBrush DoorBrush = new()
    {
        Color = Colors.VeryDarkGray,
        Style = PaintStyle.Fill
    };

    public void Render(ICanvas canvas, Depot depot)
    {
        canvas.RotateDegrees(depot.Rotation, 50, 50);

        canvas.DrawRoundRect(8, 12, BuildingRight - 8, 76, 5, 5, BuildingBrush);

        var openingTop = 50 - (_trackParameters.PlankLength / 2);
        canvas.DrawRect(TrackStart, openingTop, BuildingRight - TrackStart, _trackParameters.PlankLength, DoorBrush);

        canvas.ClipRect(new Rectangle(TrackStart, 0, 100, 100), true, false);
        _trackRenderer.DrawHorizontalPlankPath(canvas);
        _trackRenderer.DrawHorizontalTracks(canvas);
    }
}
