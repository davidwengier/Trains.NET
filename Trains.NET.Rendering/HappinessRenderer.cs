using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;
using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    internal class HappinessRenderer : IBoardRenderer, IDisposable
    {
        private readonly IGameBoard _gameBoard;
        private readonly IPixelMapper _pixelMapper;
        private readonly SKPaint _paint = new SKPaint
        {
            Color = SKColors.Cyan,
            Style = SKPaintStyle.Fill
        };

        public HappinessRenderer(IGameBoard gameBoard, IPixelMapper pixelMapper)
        {
            _gameBoard = gameBoard;
            _pixelMapper = pixelMapper;
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

                canvas.DrawRect(x, y, Game.CellSize, Game.CellSize, _paint);
            }
        }
    }
}
