using System;
using System.Collections.Generic;

namespace Trains.NET.Rendering
{
    public class TreeRenderer : ITreeRenderer
    {
        private readonly float _maxTreeSize;
        private readonly float _minTreeSize;
        private readonly int _cellSize;
        private readonly float _centerOffset;
        private readonly float _baseRadius;
        private readonly PaintBrush _baseTreeBrush;
        private readonly PaintBrush _topTreeBrush;
        private readonly Dictionary<int, IBitmap> _cachedStyles = new Dictionary<int, IBitmap>();
        private readonly IBitmapFactory _bitmapFactory;

        // Change this if you don't like the majority of tree styles
        private const int SeedOffset = 1337;

        // Change this if you want more variance/styles
        private const int MaxStyles = 100;

        public TreeRenderer(IBitmapFactory bitmapFactory, ITrackParameters trackParameters)
        {
            _cellSize = trackParameters.CellSize;
            _centerOffset = _cellSize / 2.0f;
            _baseRadius = _cellSize / 4.0f;
            _minTreeSize = _baseRadius;
            _maxTreeSize = _baseRadius * 1.25f;
            _baseTreeBrush = new PaintBrush
            {
                Color = new Color("#1B633A"),
                Style = PaintStyle.Fill
            };
            _topTreeBrush = new PaintBrush
            {
                Color = new Color("#236A42"),
                Style = PaintStyle.Fill,
                IsAntialias = true
            };
            _bitmapFactory = bitmapFactory;
        }

        public void Render(ICanvas canvas, int treeSeed)
        {
            int index = treeSeed % MaxStyles;
            if (!_cachedStyles.TryGetValue(index, out IBitmap cachedBitmap))
            {
                cachedBitmap = _bitmapFactory.CreateBitmap(_cellSize, _cellSize);
                ICanvas cachedCanvas = _bitmapFactory.CreateCanvas(cachedBitmap);

                DrawTree(cachedCanvas, index);

                _cachedStyles.Add(index, cachedBitmap);
            }

            canvas.DrawBitmap(cachedBitmap, 0, 0);
        }

        private void DrawTree(ICanvas canvas, int treeSeed)
        {
            canvas.Translate(_centerOffset, _centerOffset);

            // Let's make some repeatable numbers
            var r = new Random(SeedOffset + treeSeed);
            int circleCount = r.Next(10, 20);

            // Draw a base fill
            canvas.DrawCircle(0, 0, _baseRadius, _baseTreeBrush);

            // Draw a few base circles
            float angleOffset = r.NextFloat(0, (float)(Math.PI / 2.0f));
            DrawTreeLayer(canvas, r, circleCount, angleOffset, 1.0f, _baseTreeBrush);

            // Draw a few top circles
            angleOffset = r.NextFloat(0, (float)(Math.PI / 2.0f));
            DrawTreeLayer(canvas, r, (int)(circleCount * 0.7), angleOffset, 0.5f, _topTreeBrush);
        }

        private void DrawTreeLayer(ICanvas canvas, Random r, int circleCount, float angleOffset, float scale, PaintBrush brush)
        {
            for (int i = 0; i < circleCount; i++)
            {
                float angle = angleOffset + (float)(Math.PI * 2.0 * i / circleCount);
                float offset = r.NextFloat(_minTreeSize, _maxTreeSize);
                float radius = r.NextFloat(_cellSize / 10, 3 * _cellSize / 20);
                float x = (float)(scale * offset * Math.Cos(angle));
                float y = (float)(scale * offset * Math.Sin(angle));

                canvas.DrawCircle(x, y, radius, brush);
            }
        }
    }
}
