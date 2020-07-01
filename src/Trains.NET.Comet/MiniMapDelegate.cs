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
        private bool _redraw = true;
        private readonly IGameBoard _gameBoard;
        private readonly IPixelMapper _pixelMapper;
        private readonly ITrackParameters _trackParameters;
        private readonly SKPaint _paint = new SKPaint()
        {
            Style = SKPaintStyle.Fill,
            Color = SKColors.Gray
        };
        private readonly SKPaint _viewPortPaint = new SKPaint()
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.Green,
            StrokeWidth = 80
        };

        public MiniMapDelegate(IGameBoard gameBoard, ITrackParameters trackParameters, IPixelMapper pixelMapper)
        {
            _gameBoard = gameBoard;
            _trackParameters = trackParameters;
            _pixelMapper = pixelMapper;

            _pixelMapper.ViewPortChanged += (s, e) => _redraw = true;
            _gameBoard.TracksChanged += (s, e) => _redraw = true;
        }

        public override void Draw(SKCanvas canvas, RectangleF dirtyRect)
        {
            if (dirtyRect.IsEmpty) return;
            if (!_redraw) return;

            const int maxGridSize = PixelMapper.MaxGridSize;
            using var bitmap = new SKBitmap(maxGridSize, maxGridSize);

            using var tempCanvas = new SKCanvas(bitmap);
            tempCanvas.Clear(SKColor.Parse(Colors.VeryLightGray.HexCode));
            using var canvasWrapper = new SKCanvasWrapper(tempCanvas);

            foreach ((int, int, Track) track in _gameBoard.GetTracks())
            {
                (int x, int y) = _pixelMapper.CoordsToWorldPixels(track.Item1, track.Item2);
                tempCanvas.DrawRect(new SKRect(x, y, _trackParameters.CellSize + x, _trackParameters.CellSize + y), _paint);
            }

            tempCanvas.DrawRect(new SKRect(_pixelMapper.ViewPortX * -1, _pixelMapper.ViewPortY * -1, Math.Abs(_pixelMapper.ViewPortX) + _pixelMapper.ViewPortWidth, Math.Abs(_pixelMapper.ViewPortY) + _pixelMapper.ViewPortHeight), _viewPortPaint);

            canvas.DrawBitmap(bitmap, new SKRect(0, 0, maxGridSize, maxGridSize), new SKRect(0, 0, 100, 100));

            _redraw = false;
        }

        public override bool StartInteraction(PointF[] points)
        {
            DragInteraction(points);
            return true;
        }

        public override void DragInteraction(PointF[] points)
        {
            float x = points[0].X * (PixelMapper.MaxGridSize / 100);
            float y = points[0].Y * (PixelMapper.MaxGridSize / 100);

            x -= _pixelMapper.ViewPortWidth / 2;
            y -= _pixelMapper.ViewPortHeight / 2;

            _pixelMapper.SetViewPort((int)x, (int)y);
        }

        public void Dispose()
        {
            ((IDisposable)_paint).Dispose();
            ((IDisposable)_viewPortPaint).Dispose();
        }
    }
}
