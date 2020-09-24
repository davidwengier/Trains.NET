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
        private readonly WriteableBitmapSwapChain _swapChain;
        public bool Enabled { get; set; } = true;

        public GameElement(ISwapChain swapChain)
        {
            _swapChain = (WriteableBitmapSwapChain)swapChain;
            _designMode = DesignerProperties.GetIsInDesignMode(this);
            CompositionTarget.Rendering += CompositionTargetRendering;
        }

        private void CompositionTargetRendering(object? sender, EventArgs e)
        {
            var args = (RenderingEventArgs)e;

            if (!this.Enabled || _lastRenderingTime == args.RenderingTime)
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

            _onRenderTime.Start();

            _swapChain.SetSize((int)this.ActualWidth, (int)this.ActualHeight);

            _swapChain.PresentCurrent(currentImage => drawingContext.DrawImage(currentImage, new Rect(0, 0, this.ActualWidth, this.ActualHeight)));

            _onRenderTime.Stop();
        }
    }
}
