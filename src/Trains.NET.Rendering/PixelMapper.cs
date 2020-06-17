using System;

namespace Trains.NET.Rendering
{
    public class PixelMapper : IPixelMapper
    {
        private readonly ITrackParameters _parameters;
        private int _viewPortWidth;
        private int _viewPortHeight;
        public const int MaxGridSize = 3000;

        public int ViewPortX { get; private set; }
        public int ViewPortY { get; private set; }

        public event EventHandler? ViewPortChanged;

        public PixelMapper(ITrackParameters parameters)
        {
            _parameters = parameters;
        }

        public void SetViewPortSize(int width, int height)
        {
            _viewPortWidth = width;
            _viewPortHeight = height;
        }

        public void AdjustViewPort(int x, int y)
        {
            this.ViewPortX = Math.Max(Math.Min(this.ViewPortX + x, 0), -1 * (MaxGridSize - _viewPortWidth));
            this.ViewPortY = Math.Max(Math.Min(this.ViewPortY + y, 0), -1 * (MaxGridSize - _viewPortHeight));

            ViewPortChanged?.Invoke(this, EventArgs.Empty);
        }

        public (int, int) PixelsToCoords(int x, int y)
        {
            return ((x - this.ViewPortX) / _parameters.CellSize, (y - this.ViewPortY) / _parameters.CellSize);
        }

        public (int, int) CoordsToPixels(int column, int row)
        {
            return ((column * _parameters.CellSize) + this.ViewPortX, (row * _parameters.CellSize) + this.ViewPortY);
        }
    }
}
