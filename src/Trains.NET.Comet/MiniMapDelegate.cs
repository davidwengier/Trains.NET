using System;
using System.Drawing;
using System.Linq;
using Comet.Skia;
using SkiaSharp;
using Trains.NET.Engine;
using Trains.NET.Rendering;
using Trains.NET.Rendering.Skia;

namespace Trains.NET.Comet
{
    public class MiniMapDelegate : AbstractControlDelegate, IDisposable
    {
        private const int MiniMapCellSize = 40;
        private bool _redraw = true;
        private readonly ILayout _trackLayout;
        private readonly IPixelMapper _pixelMapper;
        private readonly ITerrainMap _terrainMap;
        private readonly SKPaint _paint = new SKPaint()
        {
            Style = SKPaintStyle.Fill,
            Color = SKColors.Black
        };
        private readonly SKPaint _viewPortPaint = new SKPaint()
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.White,
            StrokeWidth = 80
        };

        public MiniMapDelegate(ILayout trackLayout, IPixelMapper pixelMapper, ITerrainMap terrainMap)
        {
            _trackLayout = trackLayout;
            _pixelMapper = pixelMapper;
            _terrainMap = terrainMap;
            _pixelMapper.ViewPortChanged += (s, e) => _redraw = true;
            _trackLayout.CollectionChanged += (s, e) => _redraw = true;
            _terrainMap.CollectionChanged += (s, e) => _redraw = true;
        }

        public override void Draw(SKCanvas canvas, RectangleF dirtyRect)
        {
            if (dirtyRect.IsEmpty) return;
            if (!_redraw) return;

            const int maxGridSize = PixelMapper.MaxGridSize;
            using var bitmap = new SKBitmap(maxGridSize, maxGridSize, SKImageInfo.PlatformColorType, SKAlphaType.Premul);

            using var tempCanvas = new SKCanvas(bitmap);
            tempCanvas.Clear(TerrainColourLookup.DefaultColour.ToSkia());
            using var canvasWrapper = new SKCanvasWrapper(tempCanvas);

            using var paint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
            };

            foreach (Terrain terrain in _terrainMap)
            {
                paint.Color = TerrainColourLookup.GetTerrainColour(terrain).ToSkia();
                (int x, int y) = _pixelMapper.CoordsToWorldPixels(terrain.Column, terrain.Row);
                tempCanvas.DrawRect(new SKRect(x, y, MiniMapCellSize + x, MiniMapCellSize + y), paint);
            }

            foreach (Track track in _trackLayout.OfType<Track>())
            {
                (int x, int y) = _pixelMapper.CoordsToWorldPixels(track.Column, track.Row);
                tempCanvas.DrawRect(new SKRect(x, y, MiniMapCellSize + x, MiniMapCellSize + y), _paint);
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
