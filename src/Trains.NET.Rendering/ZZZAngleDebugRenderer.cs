using System;
using SkiaSharp;
using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    internal class ZZZAngleHelperRenderer : ILayerRenderer, IDisposable
    {
        private readonly IGameBoard _gameBoard;
        private readonly IPixelMapper _pixelMapper;
        private readonly ITrackParameters _parameters;
        private readonly SKPaint _paint = new SKPaint
        {
            Color = SKColors.Cyan,
            Style = SKPaintStyle.Stroke
        };
        private readonly SKPaint _text = new SKPaint
        {
            Color = SKColors.Red,
            TextAlign = SKTextAlign.Center,
            TextSize = 20
        };

        public bool Enabled { get; set; };
        public string Name => "AngleHelper";

        public ZZZAngleHelperRenderer(IGameBoard gameBoard, IPixelMapper pixelMapper, ITrackParameters parameters)
        {
            _gameBoard = gameBoard;
            _pixelMapper = pixelMapper;
            _parameters = parameters;
        }

        public void Dispose()
        {
            _paint.Dispose();
            _text.Dispose();
        }

        public void Render(SKCanvas canvas, int width, int height)
        {
            int offset = 400;
            int textOffset = 10;
            int cross = 10;
            int radius = 200;
            int circleSteps = 8;
            int pointSize = 1;

            canvas.DrawLine(offset - cross, offset, offset + cross, offset, _paint);
            canvas.DrawLine(offset, offset - cross, offset, offset + cross, _paint);
            canvas.DrawCircle(offset, offset, radius, _paint);
            for(int i=0; i< circleSteps; i++)
            {
                double angle = ((Math.PI * 2) / circleSteps) * i;
                double x = offset + (radius + textOffset) * Math.Cos(angle);
                double tx = offset + (radius) * Math.Cos(angle);
                double y = offset + (radius + textOffset) * Math.Sin(angle);
                double ty = offset + (radius ) * Math.Sin(angle);
                canvas.DrawRect((float)tx - pointSize, (float)ty - pointSize, pointSize * 2, pointSize * 2, _text);
                if (x > offset + radius / 3) _text.TextAlign = SKTextAlign.Left;
                else if (x < offset - radius / 3) _text.TextAlign = SKTextAlign.Right;
                else _text.TextAlign = SKTextAlign.Center;
                if (ty < offset - radius / 3) y -= 10;
                else if (ty > offset + radius / 3) y += 20;
                else y += 10;
                canvas.DrawText($"(X:{Math.Round(tx, 1)} Y:{Math.Round(ty, 1)} θ:{Math.Round(angle * 180.0 / Math.PI, 1)})", (float)x, (float)y, _text);
            }
        }
    }
}
