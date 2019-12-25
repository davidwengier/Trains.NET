using System;
using SkiaSharp;
using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    internal class HappinessRenderer : IBoardRenderer, IDisposable
    {
        private readonly IGameBoard _gameBoard;
        private readonly IPixelMapper _pixelMapper;
        private readonly ITrackParameters _parameters;
        private readonly SKPaint _paint = new SKPaint
        {
            Color = SKColors.Cyan,
            Style = SKPaintStyle.Fill
        };

        public bool Enabled { get; set; }
        public string Name => "Happiness";

        public HappinessRenderer(IGameBoard gameBoard, IPixelMapper pixelMapper, ITrackParameters parameters)
        {
            _gameBoard = gameBoard;
            _pixelMapper = pixelMapper;
            _parameters = parameters;
        }

        public void Dispose()
        {
            _paint.Dispose();
        }

        public void Render(SKSurface surface, int width, int height)
        {
            SKCanvas canvas = surface.Canvas;

            foreach ((int col, int row, Track track) in _gameBoard.GetTracks())
            {
                if (!track.Happy)
                {
                    continue;
                }

                (int x, int y) = _pixelMapper.CoordsToPixels(col, row);

                canvas.DrawRect(x, y, _parameters.CellSize, _parameters.CellSize, _paint);
            }
        }
    }
}
