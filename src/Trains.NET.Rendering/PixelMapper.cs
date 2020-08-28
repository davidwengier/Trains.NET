using System;

namespace Trains.NET.Rendering
{
    public class PixelMapper : IPixelMapper
    {
        public const int MaxGridSize = 3000;

        public int ViewPortX { get; private set; }
        public int ViewPortY { get; private set; }
        public int ViewPortWidth { get; private set; }
        public int ViewPortHeight { get; private set; }

        private float _gameScale = 1.0f;
        public float GameScale
        {
            get => _gameScale;
            set
            {
                _gameScale = value;
                ViewPortChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public int CellSize => (int)(40 * this.GameScale);

        public event EventHandler? ViewPortChanged;

        public void SetViewPortSize(int width, int height)
        {
            this.ViewPortWidth = width;
            this.ViewPortHeight = height;
        }

        public void SetViewPort(int x, int y)
        {
            int oldX = this.ViewPortX;
            int oldY = this.ViewPortY;
            this.ViewPortX = Math.Max(Math.Min(-x, 0), -1 * (MaxGridSize - this.ViewPortWidth));
            this.ViewPortY = Math.Max(Math.Min(-y, 0), -1 * (MaxGridSize - this.ViewPortHeight));

            if (this.ViewPortX != oldX || this.ViewPortY != oldY)
            {
                ViewPortChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public void AdjustViewPort(int x, int y)
        {
            SetViewPort(-1 * (this.ViewPortX + x), -1 * (this.ViewPortY + y));
        }

        public (int, int) ViewPortPixelsToCoords(int x, int y)
        {
            return ((x - this.ViewPortX) / this.CellSize, (y - this.ViewPortY) / this.CellSize);
        }

        public (int, int, bool) CoordsToViewPortPixels(int column, int row)
        {
            int x = (column * this.CellSize) + this.ViewPortX;
            int y = (row * this.CellSize) + this.ViewPortY;
            bool onScreen = x >= -this.CellSize &&
                            y >= -this.CellSize &&
                            x < this.ViewPortWidth + this.CellSize &&
                            y < this.ViewPortHeight + this.CellSize;
            return (x, y, onScreen);
        }

        public (int, int) WorldPixelsToCoords(int x, int y)
        {
            return (x / this.CellSize, y / this.CellSize);
        }

        public (int, int) CoordsToWorldPixels(int column, int row)
        {
            return (column * this.CellSize, row * this.CellSize);
        }

        public IPixelMapper Snapshot()
        {
            return new PixelMapper()
            {
                 ViewPortX = this.ViewPortX,
                 ViewPortY = this.ViewPortY,
                 ViewPortHeight = this.ViewPortHeight,
                 ViewPortWidth = this.ViewPortWidth,
                 GameScale = this.GameScale
            };
        }
    }
}
