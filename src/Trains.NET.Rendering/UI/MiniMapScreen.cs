using System;
using Trains.NET.Engine;

namespace Trains.NET.Rendering.UI
{
    public class MiniMapScreen : IScreen
    {
        private readonly PaintBrush _border = new PaintBrush
        {
            Color = Colors.Black,
            StrokeWidth = 2,
            Style = PaintStyle.Stroke
        };
        private readonly PaintBrush _paint = new PaintBrush()
        {
            Style = PaintStyle.Fill,
            Color = Colors.Black
        };
        private readonly PaintBrush _viewPortPaint = new PaintBrush()
        {
            Style = PaintStyle.Stroke,
            Color = Colors.White,
            StrokeWidth = 1
        };

        private readonly ITerrainMapRenderer _terrainMapRenderer;
        private readonly ILayout<Track> _trackLayout;
        private readonly IPixelMapper _pixelMapper;

        public event EventHandler? Changed;

        public MiniMapScreen(ITerrainMapRenderer terrainMapRenderer, ILayout<Track> trackLayout, IPixelMapper pixelMapper)
        {
            _terrainMapRenderer = terrainMapRenderer;
            _trackLayout = trackLayout;
            _pixelMapper = pixelMapper;

            _trackLayout.CollectionChanged += (s, e) => Changed?.Invoke(this, EventArgs.Empty);
            _pixelMapper.ViewPortChanged += (s, e) => Changed?.Invoke(this, EventArgs.Empty);
        }

        public bool HandleInteraction(int x, int y, int width, int height, MouseAction action)
        {
            if (action == MouseAction.Move)
            {
                return false;
            }

            x -= 100;
            y -= height - _pixelMapper.Rows - 100;

            if (x <= 0 || x >= _pixelMapper.Columns ||
                y <= 0 || y >= _pixelMapper.Rows)
            {
                return false;
            }

            // we're inside the minimap
            (x, y) = _pixelMapper.CoordsToWorldPixels(x, y);

            x -= _pixelMapper.ViewPortWidth / 2;
            y -= _pixelMapper.ViewPortHeight / 2;

            _pixelMapper.SetViewPort(x, y);

            return true;
        }

        public void Render(ICanvas canvas, int width, int height)
        {
            canvas.Save();

            canvas.Translate(100, height - _pixelMapper.Rows - 100);

            if (_terrainMapRenderer.TryGetTerrainImage(out IImage? terrainImage))
            {
                canvas.DrawImage(terrainImage, 0, 0);
            }
            else
            {
                canvas.Clear(TerrainColourLookup.DefaultColour);
            }

            foreach (Track track in _trackLayout)
            {
                canvas.DrawRect(track.Column, track.Row, 1, 1, _paint);
            }

            (int left, int top) = _pixelMapper.ViewPortPixelsToCoords(0, 0);
            (int right, int bottom) = _pixelMapper.ViewPortPixelsToCoords(_pixelMapper.ViewPortWidth, _pixelMapper.ViewPortHeight);

            canvas.DrawRect(left, top, right - left, bottom - top, _viewPortPaint);

            canvas.DrawRect(0, 0, _pixelMapper.Columns, _pixelMapper.Rows, _border);

            canvas.Restore();
        }
    }
}
