using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SkiaSharp;
using Trains.NET.Rendering;
using Trains.NET.Rendering.Skia;
using Trains.NET.Instrumentation;
using System;

namespace Trains
{
    public class GameElement : FrameworkElement
    {
        private readonly bool _designMode;
        private readonly IGame _game;
        private WriteableBitmap? _bitmap;
        private TimeSpan _lastRenderingTime = TimeSpan.Zero;
        private readonly PerSecondTimedStat _fps = InstrumentationBag.Add<PerSecondTimedStat>("Real-FPS");
        private readonly ElapsedMillisecondsTimedStat _drawTime = InstrumentationBag.Add<ElapsedMillisecondsTimedStat>("Real Draw Time");

        public bool Enabled { get; set; } = true;

        public GameElement(IGame game)
        {
            _designMode = DesignerProperties.GetIsInDesignMode(this);
            _game = game;
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

            _drawTime.Start();

            InvalidateVisual();

            _drawTime.Stop();

            _fps.Update();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (_designMode)
                return;

            int wpfWidth = (int)this.ActualWidth;
            int wpfHeight = (int)this.ActualHeight;


            var info = new SKImageInfo(wpfWidth, wpfHeight, SKImageInfo.PlatformColorType, SKAlphaType.Premul);

            // reset the bitmap if the size has changed
            if (_bitmap == null || info.Width != _bitmap.PixelWidth || info.Height != _bitmap.PixelHeight)
            {
                _bitmap = new WriteableBitmap(info.Width, info.Height, 96, 96, PixelFormats.Pbgra32, null);
            }

            if (_bitmap == null)
                return;

            // draw on the bitmap
            _bitmap.Lock();
            using (var surface = SKSurface.Create(info, _bitmap.BackBuffer, _bitmap.BackBufferStride))
            {
                _game.Render(new SKCanvasWrapper(surface.Canvas));
            }

            // draw the bitmap to the screen
            _bitmap.AddDirtyRect(new Int32Rect(0, 0, info.Width, info.Height));
            _bitmap.Unlock();
            drawingContext.DrawImage(_bitmap, new Rect(0, 0, this.ActualWidth, this.ActualHeight));
        }
    }
}
