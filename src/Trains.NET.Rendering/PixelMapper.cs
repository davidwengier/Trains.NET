using System;

namespace Trains.NET.Rendering
{
    public class PixelMapper : IPixelMapper
    {
        private readonly int _columns = 200;
        private readonly int _rows = 100;

        public int Columns => _columns;
        public int Rows => _rows;

        public int MaxGridWidth => _columns * this.CellSize;
        public int MaxGridHeight => _rows * this.CellSize;

        public float GameScale { get; private set; } = 1.0f;
        public int ViewPortX { get; private set; }
        public int ViewPortY { get; private set; }
        public int ViewPortWidth { get; private set; }
        public int ViewPortHeight { get; private set; }

        public int CellSize => (int)(40 * this.GameScale);

        public event EventHandler? ViewPortChanged;

        public void SetViewPortSize(int width, int height)
        {
            this.ViewPortWidth = width;
            this.ViewPortHeight = height;
            AdjustViewPort(0, 0);
        }

        public void SetViewPort(int x, int y)
        {
            int oldX = this.ViewPortX;
            int oldY = this.ViewPortY;
            this.ViewPortX = Math.Max(Math.Min(-x, 0), -1 * (this.MaxGridWidth - this.ViewPortWidth));
            this.ViewPortY = Math.Max(Math.Min(-y, 0), -1 * (this.MaxGridHeight - this.ViewPortHeight));

            if (this.ViewPortX != oldX || this.ViewPortY != oldY)
            {
                ViewPortChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public void SetViewPortCenter(int x, int y)
            => SetViewPort(x - this.ViewPortWidth / 2, y - this.ViewPortHeight / 2);

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
                GameScale = this.GameScale
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
                this.MaxGridWidth / this.GameScale * newGameScale < this.ViewPortWidth ||
                this.MaxGridHeight / this.GameScale * newGameScale < this.ViewPortHeight)
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
