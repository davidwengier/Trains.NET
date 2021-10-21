using System.Reflection;
using SkiaSharp;
using SkiaSharp.Views.Blazor;
using Trains.NET.Instrumentation;
using Trains.NET.Rendering;
using Trains.NET.Rendering.Skia;

namespace BlazingTrains.Pages
{
    public partial class Index
    {
        private SKGLView _skiaView = null!;
        private IGame _game = null!;
        private IInteractionManager _interactionManager = null!;
        private bool _hasSetContext;

        private readonly PerSecondTimedStat _fps = InstrumentationBag.Add<PerSecondTimedStat>("SkiaSharp-OnPaintSurfaceFPS");
        private readonly ElapsedMillisecondsTimedStat _renderTime = InstrumentationBag.Add<ElapsedMillisecondsTimedStat>("GameElement-GameRender");

        protected override void OnInitialized()
        {
            _game = DI.ServiceLocator.GetService<IGame>();
            _interactionManager = DI.ServiceLocator.GetService<IInteractionManager>();

            this.BeforeUnload.BeforeUnloadHandler += BeforeUnload_BeforeUnloadHandler;
        }

        protected void OnPaintSurface(SKPaintGLSurfaceEventArgs e)
        {
            if (!_hasSetContext && DI.ServiceLocator.GetService<IImageFactory>() is SKImageFactory imageFactory)
            {
                var field = typeof(SKGLView).GetField("context", BindingFlags.Instance | BindingFlags.NonPublic);
                var context = (GRContext?)field?.GetValue(_skiaView);
                if (context != null)
                {
                    imageFactory.SetContext(context);
                    _hasSetContext = true;
                }
            }
            using (_renderTime.Measure())
            {
                _game.SetSize(e.Info.Width, e.Info.Height);
                _game.Render(new SKCanvasWrapper(e.Surface.Canvas));
            }

            _fps.Update();
        }

        private void BeforeUnload_BeforeUnloadHandler(object? sender, blazejewicz.Blazor.BeforeUnload.BeforeUnloadArgs e)
        {
            _game.Dispose();
        }

        public void Dispose()
        {
            this.BeforeUnload.BeforeUnloadHandler -= BeforeUnload_BeforeUnloadHandler;
        }
    }
}
