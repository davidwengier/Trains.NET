using System;
using Trains.NET.Engine;

namespace Trains.NET.Rendering.UI
{
    [Order(10)]
    public class MiniMapScreen : IScreen, IInteractionHandler, ITogglable
    {
        private readonly PaintBrush _border = new()
        {
            Color = Colors.Black,
            StrokeWidth = 2,
            Style = PaintStyle.Stroke
        };
        private readonly PaintBrush _paint = new()
        {
            Style = PaintStyle.Fill,
            Color = Colors.Black
        };
        private readonly PaintBrush _viewPortPaint = new()
        {
            Style = PaintStyle.Stroke,
            Color = Colors.White,
            StrokeWidth = 2
        };

        private readonly ITerrainMapRenderer _terrainMapRenderer;
        private readonly ILayout<Track> _trackLayout;
        private readonly IPixelMapper _pixelMapper;
        private bool _enabled = true;

        public event EventHandler? Changed;

        public string Name => "Minimap";

        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                _enabled = value;
                Changed?.Invoke(this, EventArgs.Empty);
            }
        }

        public bool PreHandleNextClick => false;

        public MiniMapScreen(ITerrainMapRenderer terrainMapRenderer, ILayout<Track> trackLayout, IPixelMapper pixelMapper)
        {
            _terrainMapRenderer = terrainMapRenderer;
            _trackLayout = trackLayout;
            _pixelMapper = pixelMapper;

            _trackLayout.CollectionChanged += (s, e) => Changed?.Invoke(this, EventArgs.Empty);
            _pixelMapper.ViewPortChanged += (s, e) => Changed?.Invoke(this, EventArgs.Empty);
            _terrainMapRenderer.Changed += (s, e) => Changed?.Invoke(this, EventArgs.Empty);
        }

        public bool HandlePointerAction(int x, int y, int width, int height, PointerAction action)
        {
            if (!this.Enabled)
            {
                return false;
            }

            if (action == Rendering.PointerAction.Move)
            {
                return false;
            }

            x -= width - _pixelMapper.Columns - 50;
            y -= height - _pixelMapper.Rows - 50;

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
            if (!this.Enabled)
            {
                return;
            }

            canvas.Translate(width - _pixelMapper.Columns - 50, height - _pixelMapper.Rows - 50);

            canvas.DrawImage(_terrainMapRenderer.GetTerrainImage(), 0, 0);

            foreach (Track track in _trackLayout)
            {
                canvas.DrawRect(track.Column, track.Row, 1, 1, _paint);
            }

            (int left, int top) = _pixelMapper.ViewPortPixelsToCoords(0, 0);
            (int right, int bottom) = _pixelMapper.ViewPortPixelsToCoords(_pixelMapper.ViewPortWidth, _pixelMapper.ViewPortHeight);

            canvas.DrawRect(left, top, right - left, bottom - top, _viewPortPaint);

            canvas.DrawRect(0, 0, _pixelMapper.Columns, _pixelMapper.Rows, _border);
        }
    }
}
