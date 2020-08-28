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
            StrokeWidth = 1
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

            canvas.Clear(TerrainColourLookup.DefaultColour.ToSkia());

            using var paint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
            };

            foreach (Terrain terrain in _terrainMap)
            {
                paint.Color = TerrainColourLookup.GetTerrainColour(terrain).ToSkia();
                canvas.DrawRect(terrain.Column, terrain.Row, 1, 1, paint);
            }

            foreach (Track track in _trackLayout.OfType<Track>())
            {
                canvas.DrawRect(track.Column, track.Row, 1, 1, _paint);
            }

            (int left, int top) = _pixelMapper.ViewPortPixelsToCoords(0, 0);
            (int right, int bottom) = _pixelMapper.ViewPortPixelsToCoords(_pixelMapper.ViewPortWidth, _pixelMapper.ViewPortHeight);

            canvas.DrawRect(left, top, right - left, bottom - top, _viewPortPaint);

            _redraw = false;
        }

        public override bool StartInteraction(PointF[] points)
        {
            DragInteraction(points);
            return true;
        }

        public override void DragInteraction(PointF[] points)
        {
            float x = points[0].X * (PixelMapper.MaxGridSize / 75) * _pixelMapper.GameScale;
            float y = points[0].Y * (PixelMapper.MaxGridSize / 75) * _pixelMapper.GameScale;

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
