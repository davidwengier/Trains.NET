using SkiaSharp;
using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    internal class TrackLayoutRenderer : IBoardRenderer
    {
        private readonly GameBoard _gameBoard;

        public TrackLayoutRenderer(GameBoard gameBoard)
        {
            _gameBoard = gameBoard;
        }

        void IBoardRenderer.Render(SKSurface surface, int width, int height)
        {
            SKCanvas canvas = surface.Canvas;

            foreach ((int col, int row, Track track) in _gameBoard.GetTracks())
            {
                canvas.Save();

                canvas.Translate(col * Game.CellSize, row * Game.CellSize);

                using var text = new SKPaint
                {
                     Color = SKColors.Black,
                     TextSize = 32,
                     TextAlign = SKTextAlign.Center
                };
                canvas.DrawText("H", 20, 30, text);

                canvas.Restore();
            }
        }
    }
}
