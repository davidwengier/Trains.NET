using System;
using SkiaSharp;
using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    internal class TrackLayoutRenderer : IBoardRenderer
    {
        private readonly IGameBoard _gameBoard;
        private readonly ITrackRenderer _trackRenderer;
        private readonly IPixelMapper _pixelMapper;

        public TrackLayoutRenderer(IGameBoard gameBoard, ITrackRenderer trackRenderer, IPixelMapper pixelMapper)
        {
            _gameBoard = gameBoard;
            _trackRenderer = trackRenderer;
            _pixelMapper = pixelMapper;
        }

        public void Render(SKSurface surface, int width, int height)
        {
            SKCanvas canvas = surface.Canvas;

            foreach ((int col, int row, Track track) in _gameBoard.GetTracks())
            {
                canvas.Save();

                (int x, int y) = _pixelMapper.CoordsToPixels(col, row);

                canvas.Translate(x, y);

                _trackRenderer.Render(canvas, track, Game.CellSize);

                canvas.Restore();
            }
        }
    }
}
