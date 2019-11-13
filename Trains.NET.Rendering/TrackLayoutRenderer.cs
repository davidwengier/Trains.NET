using SkiaSharp;
using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    public class TrackLayoutRenderer : IBoardRenderer
    {
        private readonly GameBoard _gameBoard;
        private readonly ITrackRenderer _trackRenderer;

        public TrackLayoutRenderer(GameBoard gameBoard, ITrackRenderer trackRenderer)
        {
            _gameBoard = gameBoard;
            _trackRenderer = trackRenderer;
        }

        void IBoardRenderer.Render(SKSurface surface, int width, int height)
        {
            SKCanvas canvas = surface.Canvas;

            foreach ((int col, int row, Track track) in _gameBoard.GetTracks())
            {
                canvas.Save();

                canvas.Translate(col * Game.CellSize, row * Game.CellSize);

                _trackRenderer.Render(canvas, track, Game.CellSize);

                canvas.Restore();
            }
        }
    }
}
