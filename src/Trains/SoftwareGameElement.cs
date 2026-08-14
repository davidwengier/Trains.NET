using System.Windows.Media;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;
using Trains.NET.Instrumentation;
using Trains.NET.Rendering;
using Trains.NET.Rendering.Skia;

namespace Trains;

internal sealed class SoftwareGameElement : SKElement
{
    private static readonly TimeSpan MinimumRenderInterval = TimeSpan.FromSeconds(1d / 60);

    private readonly IGame _game;
    private TimeSpan _lastRenderingTime = TimeSpan.Zero;
    private readonly PerSecondTimedStat _wpfFps = InstrumentationBag.Add<PerSecondTimedStat>("WPF-Software-CompositionTargetFPS");
    private readonly ElapsedMillisecondsTimedStat _renderTime = InstrumentationBag.Add<ElapsedMillisecondsTimedStat>("SoftwareGameElement-GameRender");
    private readonly PerSecondTimedStat _fps = InstrumentationBag.Add<PerSecondTimedStat>("SoftwareGameElement-OnRenderFPS");

    public SoftwareGameElement(IGame game)
    {
        _game = game;
        CompositionTarget.Rendering += CompositionTargetRendering;
    }

    private void CompositionTargetRendering(object? sender, EventArgs e)
    {
        var args = (RenderingEventArgs)e;

        if (args.RenderingTime - _lastRenderingTime < MinimumRenderInterval)
        {
            return;
        }

        _lastRenderingTime = args.RenderingTime;

        InvalidateVisual();

        _wpfFps.Update();
    }

    protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
    {
        using (_renderTime.Measure())
        {
            _game.SetSize(e.Info.Width, e.Info.Height);
            _game.Render(new SKCanvasWrapper(e.Surface.Canvas));
        }

        _fps.Update();
    }
}
