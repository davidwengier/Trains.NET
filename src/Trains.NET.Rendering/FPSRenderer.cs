using System;
using System.Diagnostics;
using SkiaSharp;

namespace Trains.NET.Rendering
{
    internal class FPSRenderer : ILayerRenderer, IDisposable
    {
        private readonly Stopwatch _stopwatch = Stopwatch.StartNew();

        private readonly SKPaint _paint = new SKPaint
        {
            Color = SKColors.Purple,
            TextSize = 20,
            TextAlign = SKTextAlign.Left,
        };
        private long _lastDrawTime;

        public bool Enabled { get; set; } = true;

        public string Name => "FPS";

        public void Dispose()
        {
            _paint.Dispose();
        }

        public void Render(SKCanvas canvas, int width, int height)
        {
            var now = _stopwatch.ElapsedMilliseconds;
            var timeSinceLastUpdate = now - _lastDrawTime;
            _lastDrawTime = now;

            canvas.DrawText((1000 / timeSinceLastUpdate) + " FPS", 0, 20, _paint);
        }
    }
}
