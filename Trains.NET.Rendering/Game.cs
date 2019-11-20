using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using SkiaSharp;
using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    internal class Game : IGame
    {
        public const int CellSize = 40;

        private int _width;
        private int _height;

        private readonly IGameBoard _gameBoard;
        private readonly IEnumerable<IBoardRenderer> _boardRenderers;
        private readonly IPixelMapper _pixelMapper;

        public Tool CurrentTool { get; set; }

        public Game(IGameBoard gameBoard, IEnumerable<IBoardRenderer> boardRenderers, IPixelMapper pixelMapper)
        {
            _gameBoard = gameBoard;
            _boardRenderers = boardRenderers;
            _pixelMapper = pixelMapper;
        }

        public void SetSize(int width, int height)
        {
            (int columns, int rows) = _pixelMapper.PixelsToCoords(width, height);
            columns = Math.Max(columns, 1);
            rows = Math.Max(rows, 1);

            (_width, _height) = _pixelMapper.CoordsToPixels(columns, rows);

            _gameBoard.Columns = columns;
            _gameBoard.Rows = rows;
        }

        public void Render(SKSurface surface)
        {
            if (surface is null)
            {
                throw new ArgumentNullException(nameof(surface));
            }

            SKCanvas canvas = surface.Canvas;

            canvas.Translate(1, 1);
            canvas.Clear(SKColors.White);
            canvas.ClipRect(new SKRect(0, 0, _width + 2, _height + 2), SKClipOperation.Intersect, false);

            foreach (IBoardRenderer renderer in _boardRenderers)
            {
                canvas.Save();
                renderer.Render(surface, _width, _height);
                canvas.Restore();
            }
        }

        public void OnMouseDown(int x, int y)
        {
            (int column, int row) = _pixelMapper.PixelsToCoords(x, y);

            if (this.CurrentTool == Tool.Track)
            {
                _gameBoard.AddTrack(column, row);
            }
        }
    }
}
