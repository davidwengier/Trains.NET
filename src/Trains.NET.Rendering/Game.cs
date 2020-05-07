using System;
using System.Collections.Generic;
using System.Linq;
using Trains.NET.Engine;
using Trains.NET.Instrumentation;

namespace Trains.NET.Rendering
{
    internal class Game : IGame
    {
        private int _width;
        private int _height;

        private readonly IGameBoard _gameBoard;
        private readonly IEnumerable<ILayerRenderer> _boardRenderers;
        private readonly IPixelMapper _pixelMapper;

        private readonly PerSecondTimedStat _skiaFps = InstrumentationBag.Add<PerSecondTimedStat>("SkiaFPS");
        private readonly ElapsedMillisecondsTimedStat _skiaDrawTime = InstrumentationBag.Add<ElapsedMillisecondsTimedStat>("SkiaDrawTime");
        private readonly Dictionary<ILayerRenderer, ElapsedMillisecondsTimedStat> _renderLayerDrawTimes;

        public Game(IGameBoard gameBoard, OrderedList<ILayerRenderer> boardRenderers, IPixelMapper pixelMapper)
        {
            _gameBoard = gameBoard;
            _boardRenderers = boardRenderers;
            _pixelMapper = pixelMapper;

            _renderLayerDrawTimes = _boardRenderers.ToDictionary(x => x, x => InstrumentationBag.Add<ElapsedMillisecondsTimedStat>(x.Name.Replace(" ", "") + "DrawTime"));
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

        public void Render(ICanvas canvas)
        {
            _skiaDrawTime.Start();
            if (canvas is null)
            {
                throw new ArgumentNullException(nameof(canvas));
            }

            canvas.Save();
            canvas.Translate(1, 1);
            canvas.Clear(Colors.White);
            canvas.ClipRect(new Rectangle(0, 0, _width + 2, _height + 2), ClipOperation.Intersect, false);

            foreach (ILayerRenderer renderer in _boardRenderers)
            {
                if (!renderer.Enabled)
                {
                    continue;
                }
                _renderLayerDrawTimes[renderer].Start();
                canvas.Save();
                renderer.Render(canvas, _width, _height);
                canvas.Restore();
                _renderLayerDrawTimes[renderer].Stop();
            }
            canvas.Restore();
            _skiaDrawTime.Stop();
            _skiaFps.Update();
        }
    }
}
