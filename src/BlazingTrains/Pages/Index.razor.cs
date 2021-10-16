﻿using SkiaSharp.Views.Blazor;
using Trains.NET.Instrumentation;
using Trains.NET.Rendering;
using Trains.NET.Rendering.Skia;

namespace BlazingTrains.Pages
{
    public partial class Index
    {
        private SKCanvasView _skiaView = null!;
        private IGame _game = null!;
        private IInteractionManager _interactionManager = null!;

        private readonly PerSecondTimedStat _fps = InstrumentationBag.Add<PerSecondTimedStat>("SkiaSharp-OnPaintSurfaceFPS");
        private readonly ElapsedMillisecondsTimedStat _renderTime = InstrumentationBag.Add<ElapsedMillisecondsTimedStat>("GameElement-GameRender");

        protected override void OnInitialized()
        {
            _game = DI.ServiceLocator.GetService<IGame>();
            _interactionManager = DI.ServiceLocator.GetService<IInteractionManager>();

            this.BeforeUnload.BeforeUnloadHandler += BeforeUnload_BeforeUnloadHandler;
        }

        protected void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
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