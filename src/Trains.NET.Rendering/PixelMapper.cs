using System;

namespace Trains.NET.Rendering
{
    public class PixelMapper : IPixelMapper
    {
        public int ViewPortX { get; private set; }
        public int ViewPortY { get; private set; }
        public int ViewPortWidth { get; private set; }
        public int ViewPortHeight { get; private set; }
        public float GameScale { get; private set; } = 1.0f;

        public int MaxGridSize => _columns * this.CellSize;
        public int CellSize => (int)(40 * this.GameScale);

        private int _columns;
        private int _rows;

        public event EventHandler? ViewPortChanged;

        public void Initialize(int columns, int rows)
        {
            _columns = columns;
            _rows = rows;
        }

        public void SetViewPortSize(int width, int height)
        {
            this.ViewPortWidth = width;
            this.ViewPortHeight = height;
        }

        public void SetViewPort(int x, int y)
        {
            int oldX = this.ViewPortX;
            int oldY = this.ViewPortY;
            this.ViewPortX = Math.Max(Math.Min(-x, 0), -1 * (this.MaxGridSize - this.ViewPortWidth));
            this.ViewPortY = Math.Max(Math.Min(-y, 0), -1 * (this.MaxGridSize - this.ViewPortHeight));

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
            bool onScreen = x > -this.CellSize &&
                            y > -this.CellSize &&
                            x <= this.ViewPortWidth &&
                            y <= this.ViewPortHeight;
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
                 GameScale = this.GameScale,
                 _columns = _columns,
                 _rows = _rows
            };
        }

        private (float, float) GetScaledViewPortSize()
            => GetScaledViewPortSize(this.GameScale);

        private (float, float) GetScaledViewPortSize(float scale)
            => (this.ViewPortWidth / scale,
                this.ViewPortHeight / scale);

        public void AdjustGameScale(float delta)
        {
            float newGameScale = this.GameScale * delta;

            // Check to see if it is TOO FAR!
            if (newGameScale < 0.1 ||
                this.MaxGridSize / this.GameScale * newGameScale < this.ViewPortWidth ||
                this.MaxGridSize / this.GameScale * newGameScale < this.ViewPortHeight)
            {
                return;
            }

            // Viewport X & Y will be negative, as they are canvas transations, so swap em!
            float currentX = -this.ViewPortX / this.GameScale;
            float currentY = -this.ViewPortY / this.GameScale;

            (float svpWidth, float svpHeight) = GetScaledViewPortSize();

            float currentCenterX = currentX + svpWidth / 2.0f;
            float currentCenterY = currentY + svpHeight / 2.0f;

            (float newSvpWidth, float newSvpHeight) = GetScaledViewPortSize(newGameScale);

            float newX = currentCenterX - newSvpWidth / 2.0f;
            float newY = currentCenterY - newSvpHeight / 2.0f;

            this.ViewPortX = -(int)Math.Round(newX * newGameScale);
            this.ViewPortY = -(int)Math.Round(newY * newGameScale);

            this.GameScale = newGameScale;

            AdjustViewPort(0, 0);

            ViewPortChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
