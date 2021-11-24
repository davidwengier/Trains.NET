using Trains.NET.Engine;

namespace Trains.NET.Rendering.UI;

public class TrackButton : ButtonBase
{
    private readonly Track _track;
    private readonly IEnumerable<IStaticEntityRenderer<Track>> _renderers;

    public TrackButton(Track track, Func<bool> isActive, Action onClick, IEnumerable<IStaticEntityRenderer<Track>> renderers)
        : base(isActive, onClick)
    {
        _track = track;
        _renderers = renderers;
    }

    public override int GetMinimumWidth(ICanvas canvas) => 40;

    protected override void RenderButtonLabel(ICanvas canvas)
    {
        foreach (var renderer in _renderers)
        {
            if (renderer.ShouldRender(_track))
            {
                float scale = 32 / 100.0f;
                using (canvas.Scope())
                {
                    canvas.Translate(4, 4);
                    canvas.Scale(scale, scale);
                    renderer.Render(canvas, _track);
                }
            }
        }
    }
}
