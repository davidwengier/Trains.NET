using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trains.NET.Engine;
using Trains.NET.Instrumentation;
using Trains.NET.Rendering.LayerRenderer;

namespace Trains.NET.Rendering
{
    internal class Game : IGame
    {
        private bool _needsBufferReset;
        private int _width;
        private int _height;
        private readonly IGameBoard _gameBoard;
        private readonly IEnumerable<ILayerRenderer> _boardRenderers;
        private readonly IPixelMapper _pixelMapper;
        private readonly IBitmapFactory _bitmapFactory;
        private readonly PerSecondTimedStat _skiaFps = InstrumentationBag.Add<PerSecondTimedStat>("Draw-FPS-Skia");
        private readonly ElapsedMillisecondsTimedStat _skiaDrawTime = InstrumentationBag.Add<ElapsedMillisecondsTimedStat>("Draw-Skia-AllUp");
        private readonly ElapsedMillisecondsTimedStat _skiaClearTime = InstrumentationBag.Add<ElapsedMillisecondsTimedStat>("Draw-Skia-Clear");
        private readonly ElapsedMillisecondsTimedStat _gameBufferReset = InstrumentationBag.Add<ElapsedMillisecondsTimedStat>("Draw-Game-BufferReset");
        private readonly Dictionary<ILayerRenderer, ElapsedMillisecondsTimedStat> _renderLayerDrawTimes;
        private readonly Dictionary<ILayerRenderer, ElapsedMillisecondsTimedStat> _renderCacheDrawTimes;
        private readonly Dictionary<ILayerRenderer, IBitmap> _bitmapBuffer = new();

        public Game(IGameBoard gameBoard, OrderedList<ILayerRenderer> boardRenderers, IPixelMapper pixelMapper, IBitmapFactory bitmapFactory)
        {
            _gameBoard = gameBoard;
            _boardRenderers = boardRenderers;
            _pixelMapper = pixelMapper;
            _bitmapFactory = bitmapFactory;
            _renderLayerDrawTimes = _boardRenderers.ToDictionary(x => x, x => InstrumentationBag.Add<ElapsedMillisecondsTimedStat>(GetLayerDiagnosticsName(x)));
            _renderCacheDrawTimes = _boardRenderers.Where(x => x is ICachableLayerRenderer).ToDictionary(x => x, x => InstrumentationBag.Add<ElapsedMillisecondsTimedStat>("Draw-Cache-" + x.Name.Replace(" ", "")));
            _pixelMapper.ViewPortChanged += (s, e) => _needsBufferReset = true;
        }

        private static string GetLayerDiagnosticsName(ILayerRenderer layerRenderer)
        {
            var sb = new StringBuilder("Draw-Layer-");
            sb.Append(layerRenderer.Name.Replace(" ", ""));
            if(layerRenderer is ICachableLayerRenderer)
            {
                sb.Append("[Cached]");
            }
            return sb.ToString();
        }

        public void SetSize(int width, int height)
        {
            (int columns, int rows) = _pixelMapper.ViewPortPixelsToCoords(width, height);
            columns = Math.Max(columns, 1);
            rows = Math.Max(rows, 1);

            (_width, _height) = _pixelMapper.CoordsToViewPortPixels(columns, rows);

            _pixelMapper.SetViewPortSize(_width, _height);

            _gameBoard.Columns = columns;
            _gameBoard.Rows = rows;

            ResetBuffers();
        }

        private void ResetBuffers()
        {
            foreach (IBitmap bitmap in _bitmapBuffer.Values)
            {
                bitmap.Dispose();
            }
            _bitmapBuffer.Clear();
        }

        public void Render(ICanvas canvas)
        {
            if (_width == 0 || _height == 0) return;

            _skiaDrawTime.Start();
            if (canvas is null)
            {
                throw new ArgumentNullException(nameof(canvas));
            }

            canvas.Save();

            _skiaClearTime.Start();
            canvas.Clear(Colors.VeryLightGray);
            _skiaClearTime.Stop();

            foreach (ILayerRenderer renderer in _boardRenderers)
            {
                if (!renderer.Enabled)
                {
                    continue;
                }
                canvas.Save();

                if (renderer is ICachableLayerRenderer cachable)
                {
                    if (cachable.IsDirty || !_bitmapBuffer.ContainsKey(renderer))
                    {
                        _renderCacheDrawTimes[renderer].Start();
                        if (!_bitmapBuffer.TryGetValue(renderer, out IBitmap bitmap))
                        {
                            bitmap = _bitmapFactory.CreateBitmap(_width, _height);
                        }
                        ICanvas layerCanvas = _bitmapFactory.CreateCanvas(bitmap);
                        layerCanvas.Clear(Colors.Empty);
                        renderer.Render(layerCanvas, _width, _height);
                        _bitmapBuffer[renderer] = bitmap;
                        layerCanvas.Dispose();
                        _renderCacheDrawTimes[renderer].Stop();
                    }

                    _renderLayerDrawTimes[renderer].Start();
                    canvas.DrawBitmap(_bitmapBuffer[renderer], 0, 0);
                    _renderLayerDrawTimes[renderer].Stop();
                }
                else
                {
                    _renderLayerDrawTimes[renderer].Start();
                    renderer.Render(canvas, _width, _height);
                    _renderLayerDrawTimes[renderer].Stop();
                }
                canvas.Restore();
            }
            canvas.Restore();

            if (_needsBufferReset)
            {
                _gameBufferReset.Start();
                ResetBuffers();
                _needsBufferReset = false;
                _gameBufferReset.Stop();
            }

            _skiaDrawTime.Stop();
            _skiaFps.Update();
        }

        public void AdjustViewPortIfNecessary()
        {
            foreach (IMovable? vehicle in _gameBoard.GetMovables())
            {
                if (vehicle.Follow)
                {
                    (int x, int y) = _pixelMapper.CoordsToViewPortPixels(vehicle.Column, vehicle.Row);

                    double easing = 10;
                    int adjustX = Convert.ToInt32(((_pixelMapper.ViewPortWidth / 2) - x) / easing);
                    int adjustY = Convert.ToInt32(((_pixelMapper.ViewPortHeight / 2) - y) / easing);

                    if (adjustX != 0 || adjustY != 0)
                    {
                        _pixelMapper.AdjustViewPort(adjustX, adjustY);
                    }
                    break;
                }
            }
        }
    }
}
