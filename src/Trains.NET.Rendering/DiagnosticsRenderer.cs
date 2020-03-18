using System;
using System.Diagnostics;
using System.Linq;
using SkiaSharp;
using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    [Order(1000)]
    internal class DiagnosticsRenderer : ILayerRenderer, IDisposable
    {
        private long _lastDrawTime;
        private readonly IGameBoard _gameBoard;
        private readonly Stopwatch _stopwatch = Stopwatch.StartNew();
        private readonly SKPaint _paint = new SKPaint
        {
            Color = SKColors.Purple,
            TextSize = 20,
            TextAlign = SKTextAlign.Left,
        };

        public bool Enabled { get; set; } = true;

        public string Name => "Diagnostics";

        public DiagnosticsRenderer(IGameBoard gameBoard)
        {
            _gameBoard = gameBoard;
        }

        public void Dispose()
        {
            _paint.Dispose();
        }

        public void Render(SKCanvas canvas, int width, int height)
        {
            long now = _stopwatch.ElapsedMilliseconds;
            long timeSinceLastUpdate = now - _lastDrawTime;
            _lastDrawTime = now;

            int y = 1;
            canvas.DrawText((1000 / timeSinceLastUpdate) + " FPS", 0, (y++)*25, _paint);

            canvas.DrawText(_gameBoard.GetTracks().Count() + " Tracks", 0, (y++) * 25, _paint);

            canvas.DrawText(_gameBoard.GetMovables().Count() + " Trains", 0, (y++) * 25, _paint);
        }
    }
}
