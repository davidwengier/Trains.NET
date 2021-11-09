using Microsoft.AspNetCore.Components.Web;
using SkiaSharp;
using SkiaSharp.Views.Blazor;
using Trains.NET.Instrumentation;
using Trains.NET.Rendering;
using Trains.NET.Rendering.Skia;

namespace BlazingTrains.Pages;

public partial class Index
{
    private IGame _game = null!;
    private IInteractionManager _interactionManager = null!;

    private readonly PerSecondTimedStat _fps = InstrumentationBag.Add<PerSecondTimedStat>("SkiaSharp-OnPaintSurfaceFPS");
    private readonly ElapsedMillisecondsTimedStat _renderTime = InstrumentationBag.Add<ElapsedMillisecondsTimedStat>("GameElement-GameRender");

    protected override async Task OnInitializedAsync()
    {
        _game = DI.ServiceLocator.GetService<IGame>();
        _interactionManager = DI.ServiceLocator.GetService<IInteractionManager>();

        await _game.InitializeAsync(200, 200);
    }

    private void OnPaintSurface(SKPaintGLSurfaceEventArgs e)
    {
        using (_renderTime.Measure())
        {
            _game.SetSize(e.Info.Width, e.Info.Height);
            if (e.Surface.Context is GRContext context && context != null)
            {
                // Set the context so all rendering happens in the same place
                _game.SetContext(new SKContextWrapper(context));
            }
            _game.Render(new SKCanvasWrapper(e.Surface.Canvas));
        }

        _fps.Update();
    }

    private void OnPointerDown(PointerEventArgs e)
    {
        if (e.Buttons == 1)
        {
            _interactionManager.PointerClick((int)e.OffsetX, (int)e.OffsetY);
        }
        else if (e.Buttons == 2)
        {
            _interactionManager.PointerAlternateClick((int)e.OffsetX, (int)e.OffsetY);
        }
    }

    private void OnPointerMove(PointerEventArgs e)
    {
        if (e.Buttons == 1)
        {
            _interactionManager.PointerDrag((int)e.OffsetX, (int)e.OffsetY);
        }
        else if (e.Buttons == 2)
        {
            _interactionManager.PointerAlternateDrag((int)e.OffsetX, (int)e.OffsetY);
        }
        else
        {
            _interactionManager.PointerMove((int)e.OffsetX, (int)e.OffsetY);
        }
    }

    private void OnPointerUp(PointerEventArgs e)
    {
        _interactionManager.PointerRelease((int)e.OffsetX, (int)e.OffsetY);
    }

    private void OnTouchStart(TouchEventArgs e)
    {
        var touch = e.Touches.FirstOrDefault();
        if (touch is null)
            return;

        if (e.Touches.Length == 2)
        {
            _interactionManager.PointerAlternateClick((int)touch.ClientX, (int)touch.ClientY);
        }
    }

    private void OnTouchMove(TouchEventArgs e)
    {
        var touch = e.Touches.FirstOrDefault();
        if (touch is null)
            return;

        if (e.Touches.Length == 1)
        {
            _interactionManager.PointerDrag((int)touch.ClientX, (int)touch.ClientY);
        }
        else if (e.Touches.Length == 2)
        {
            _interactionManager.PointerAlternateDrag((int)touch.ClientX, (int)touch.ClientY);
        }
    }

    private void OnMouseWheel(WheelEventArgs e)
    {
        if (e.DeltaY < 0)
        {
            _interactionManager.PointerZoomIn((int)e.ClientX, (int)e.ClientY);
        }
        else
        {
            _interactionManager.PointerZoomOut((int)e.ClientX, (int)e.ClientY);
        }
    }
}
