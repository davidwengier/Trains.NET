using System;

namespace Trains.NET.Rendering
{
    public class PixelMapper : IPixelMapper
    {
        private readonly ITrackParameters _parameters;
        private const int MaxGridSize = -10000;

        public int ViewPortX { get; private set; }
        public int ViewPortY { get; private set; }

        public event EventHandler? ViewPortChanged;

        public PixelMapper(ITrackParameters parameters)
        {
            _parameters = parameters;
        }

        public void AdjustViewPort(int x, int y)
        {
            this.ViewPortX = Math.Max(Math.Min(this.ViewPortX + x, 0), MaxGridSize);
            this.ViewPortY = Math.Max(Math.Min(this.ViewPortY + y, 0), MaxGridSize);

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
