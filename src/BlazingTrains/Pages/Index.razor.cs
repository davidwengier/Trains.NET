using SkiaSharp.Views.Blazor;
using Trains.NET.Rendering;
using Trains.NET.Rendering.Skia;

namespace BlazingTrains.Pages
{
    public partial class Index
    {
        private SKCanvasView _skiaView = null!;
        private IGame _game = null!;
        private IInteractionManager _interactionManager = null!;
        protected override void OnInitialized()
        {
            _game = DI.ServiceLocator.GetService<IGame>();
            _interactionManager = DI.ServiceLocator.GetService<IInteractionManager>();
        }

        protected void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            _game.SetSize(e.Info.Width, e.Info.Height);
            _game.Render(new SKCanvasWrapper(e.Surface.Canvas));
        }
    }
}
