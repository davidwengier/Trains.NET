using Trains.NET.Engine;

namespace Trains.NET.Rendering.Tracks;

public class CrossTrackRenderer(SingleTrackRenderer trackRenderer) : SpecializedEntityRenderer<CrossTrack, Track>
{
    private readonly SingleTrackRenderer _trackRenderer = trackRenderer;

    protected override void Render(ICanvas canvas, CrossTrack item)
    {
        _trackRenderer.DrawCross(canvas);
    }
}
