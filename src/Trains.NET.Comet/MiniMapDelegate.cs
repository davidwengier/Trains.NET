using System;
using System.Drawing;
using Comet.Skia;
using SkiaSharp;
using Trains.NET.Engine;
using Trains.NET.Rendering;
using Trains.NET.Rendering.Skia;

namespace Trains.NET.Comet
{
    internal class MiniMapDelegate : AbstractControlDelegate, IDisposable
    {
        private readonly IGameBoard _gameBoard;
        private readonly IPixelMapper _pixelMapper;
        private readonly IPixelMapper _gamePixelMapper;
        private readonly ITrackParameters _trackParameters;
        private readonly SKPaint _paint = new SKPaint()
        {
            Style = SKPaintStyle.Fill,
            Color = SKColors.Gray
        };

        public MiniMapDelegate(IGameBoard gameBoard, ITrackParameters trackParameters, IPixelMapper pixelMapper)
        {
            _gameBoard = gameBoard;
            _trackParameters = trackParameters;

            _pixelMapper = new PixelMapper(_trackParameters);
            _gamePixelMapper = pixelMapper;

            _gameBoard.TracksChanged += (s, e) => Invalidate();
        }

        public override void Draw(SKCanvas canvas, RectangleF dirtyRect)
        {
            const int maxGridSize = PixelMapper.MaxGridSize;
            using var bitmap = new SKBitmap(maxGridSize, maxGridSize);

            using var tempCanvas = new SKCanvas(bitmap);
            tempCanvas.Clear(SKColor.Parse(Colors.VeryLightGray.HexCode));
            using var canvasWrapper = new SKCanvasWrapper(tempCanvas);
            int oldPadding = _trackParameters.PlankPadding;
            int oldWidth = _trackParameters.PlankWidth;
            _trackParameters.PlankPadding = 1;
            _trackParameters.PlankWidth = 20;

            foreach (var track in _gameBoard.GetTracks())
            {
                (var x, var y) = _pixelMapper.CoordsToPixels(track.Item1, track.Item2);
                tempCanvas.DrawRect(new SKRect(x, y, _trackParameters.CellSize + x, _trackParameters.CellSize + y), _paint);
            }

            _trackParameters.PlankPadding = oldPadding;
            _trackParameters.PlankWidth = oldWidth;
            canvas.DrawBitmap(bitmap, new SKRect(0, 0, maxGridSize, maxGridSize), new SKRect(0, 0, 100, 100));
        }

        public override bool StartInteraction(PointF[] points)
        {
            DragInteraction(points);
            return true;
        }

        public override void DragInteraction(PointF[] points)
        {
            var x = points[0].X * (PixelMapper.MaxGridSize / 100);
            var y = points[0].Y * (PixelMapper.MaxGridSize / 100);

            _gamePixelMapper.SetViewPort((int)x, (int)y);
        }

        public void Dispose()
        {
            ((IDisposable)_paint).Dispose();
        }
    }
}
