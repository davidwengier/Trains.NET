using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using Trains.NET.Instrumentation;
using System;
using Trains.NET.Rendering;

namespace Trains
{
    public class GameElement : FrameworkElement
    {
        private readonly bool _designMode;
        private TimeSpan _lastRenderingTime = TimeSpan.Zero;
        private readonly ElapsedMillisecondsTimedStat _onRenderTime = InstrumentationBag.Add<ElapsedMillisecondsTimedStat>("GameElement-OnRender");
        private readonly IGame _game;

        public GameElement(IGame game)
        {
            _game = game;
            _designMode = DesignerProperties.GetIsInDesignMode(this);
            CompositionTarget.Rendering += CompositionTargetRendering;
        }

        private void CompositionTargetRendering(object? sender, EventArgs e)
        {
            var args = (RenderingEventArgs)e;

            if (_lastRenderingTime == args.RenderingTime)
            {
                return;
            }

            _lastRenderingTime = args.RenderingTime;

            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (_designMode)
                return;

            using (_onRenderTime.Measure())
            {
                _game.Render(swapChain =>
                {
                    var writeableBitmapSwapChain = (WriteableBitmapSwapChain)swapChain;

                    writeableBitmapSwapChain.SetSize((int)this.ActualWidth, (int)this.ActualHeight);

                    writeableBitmapSwapChain.PresentCurrent(currentImage => drawingContext.DrawImage(currentImage, new Rect(0, 0, this.ActualWidth, this.ActualHeight)));
                });
            }
        }
    }
}
