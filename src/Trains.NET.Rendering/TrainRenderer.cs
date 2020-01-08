using System;
using SkiaSharp;
using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    internal class TrainRenderer : ITrainRenderer, IDisposable
    {
        private readonly IPixelMapper _pixelMapper;
        private readonly ITrackParameters _trackParameters;
        private readonly SKPaint _paint = new SKPaint
        {
            Color = SKColors.Red,
            Style = SKPaintStyle.Fill
        };

        public TrainRenderer(IPixelMapper pixelMapper, ITrackParameters trackParameters)
        {
            _pixelMapper = pixelMapper;
            _trackParameters = trackParameters;
        }

        public void Dispose()
        {
            _paint.Dispose();
        }

        public void Render(SKCanvas canvas, Train train)
        {
            (int x, int y) = _pixelMapper.CoordsToPixels(train.Column, train.Row);

            canvas.DrawRect(x + (_trackParameters.CellSize / 2) - 5, y + (_trackParameters.CellSize / 2) - 5, 10, 10, _paint);
        }
    }
}
