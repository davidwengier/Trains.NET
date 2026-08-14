using System.Windows;
using System.Windows.Media;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;
using Trains.NET.Instrumentation;
using Trains.NET.Rendering;
using Trains.NET.Rendering.Skia;

namespace Trains;

public class GameElement : SKGLElement
{
    private readonly IGame _game;
    private TimeSpan _lastRenderingTime = TimeSpan.Zero;
    private readonly PerSecondTimedStat _wpfFps = InstrumentationBag.Add<PerSecondTimedStat>("WPF-CompositionTargetFPS");
    private readonly ElapsedMillisecondsTimedStat _renderTime = InstrumentationBag.Add<ElapsedMillisecondsTimedStat>("GameElement-GameRender");
    private readonly PerSecondTimedStat _fps = InstrumentationBag.Add<PerSecondTimedStat>("GameElement-OnRenderFPS");

    public bool Enabled { get; set; } = true;

    public GameElement(IGame game)
    {
        _game = game;
        CompositionTarget.Rendering += CompositionTargetRendering;
    }

    private void CompositionTargetRendering(object? sender, EventArgs e)
    {
        var args = (RenderingEventArgs)e;

        if (!Enabled || _lastRenderingTime == args.RenderingTime)
        {
            return;
        }

        _lastRenderingTime = args.RenderingTime;

        InvalidateVisual();

        _wpfFps.Update();
    }

    protected override void OnPaintSurface(SKPaintGLSurfaceEventArgs e)
    {
        using (_renderTime.Measure())
        {
            _game.SetSize(e.Info.Width, e.Info.Height);
            _game.SetContext(new SKContextWrapper(GRContext));
            _game.Render(new SKCanvasWrapper(e.Surface.Canvas));
        }

        _fps.Update();
    }

    public (int X, int Y) ToPixels(Point point)
    {
        var source = PresentationSource.FromVisual(this);
        if (source is null)
        {
            return ((int)point.X, (int)point.Y);
        }

        var pixelPoint = source.CompositionTarget.TransformToDevice.Transform(point);
        return ((int)pixelPoint.X, (int)pixelPoint.Y);
    }
}
