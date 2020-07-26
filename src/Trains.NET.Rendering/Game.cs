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
        private const int RenderInterval = 16;

        private readonly object _bufferLock = new object();
        private IImage? _backBuffer;

        private bool _needsBufferReset;
        private int _width;
        private int _height;
        private readonly IGameBoard _gameBoard;
        private readonly IEnumerable<ILayerRenderer> _boardRenderers;
        private readonly IPixelMapper _pixelMapper;
        private readonly IImageFactory _imageFactory;
        private readonly PerSecondTimedStat _skiaFps = InstrumentationBag.Add<PerSecondTimedStat>("Draw-FPS-Skia");
        private readonly ElapsedMillisecondsTimedStat _skiaDrawTime = InstrumentationBag.Add<ElapsedMillisecondsTimedStat>("Draw-Skia-AllUp");
        private readonly ElapsedMillisecondsTimedStat _skiaClearTime = InstrumentationBag.Add<ElapsedMillisecondsTimedStat>("Draw-Skia-Clear");
        private readonly ElapsedMillisecondsTimedStat _gameBufferReset = InstrumentationBag.Add<ElapsedMillisecondsTimedStat>("Draw-Game-BufferReset");
        private readonly Dictionary<ILayerRenderer, ElapsedMillisecondsTimedStat> _renderLayerDrawTimes;
        private readonly Dictionary<ILayerRenderer, ElapsedMillisecondsTimedStat> _renderCacheDrawTimes;
        private readonly Dictionary<ILayerRenderer, IImage> _imageBuffer = new();
        private readonly ITimer _renderLoop;

        public Game(IGameBoard gameBoard, OrderedList<ILayerRenderer> boardRenderers, IPixelMapper pixelMapper, IImageFactory imageFactory, ITimer renderLoop)
        {
            _gameBoard = gameBoard;
            _boardRenderers = boardRenderers;
            _pixelMapper = pixelMapper;
            _imageFactory = imageFactory;
            _renderLayerDrawTimes = _boardRenderers.ToDictionary(x => x, x => InstrumentationBag.Add<ElapsedMillisecondsTimedStat>(GetLayerDiagnosticsName(x)));
            _renderCacheDrawTimes = _boardRenderers.Where(x => x is ICachableLayerRenderer).ToDictionary(x => x, x => InstrumentationBag.Add<ElapsedMillisecondsTimedStat>("Draw-Cache-" + x.Name.Replace(" ", "")));
            _pixelMapper.ViewPortChanged += (s, e) => _needsBufferReset = true;

            _renderLoop = renderLoop;
            _renderLoop.Elapsed += (s, e) => DrawFrame();
            _renderLoop.Interval = RenderInterval;
            _renderLoop.Start();
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
            foreach (IImage image in _imageBuffer.Values)
            {
                image.Dispose();
            }
            _imageBuffer.Clear();
        }

        public void Render(ICanvas canvas)
        {
            lock (_bufferLock)
            {
                if (_backBuffer != null)
                {
                    canvas.DrawImage(_backBuffer, 0, 0);
                }
            }
        }

        public void DrawFrame()
        {
            if (_width == 0 || _height == 0) return;

            AdjustViewPortIfNecessary();

            using IImageCanvas? imageCanvas = _imageFactory.CreateImageCanvas(_width, _height);

            var canvas = imageCanvas.Canvas;
            RenderFrame(canvas);

            var oldBuffer = _backBuffer;
            lock (_bufferLock)
            {
                _backBuffer = imageCanvas.Render();
            }
            oldBuffer?.Dispose();
        }

        private void RenderFrame(ICanvas? canvas)
        {
            if (canvas is null)
            {
                throw new ArgumentNullException(nameof(canvas));
            }

            _skiaDrawTime.Start();

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
                    if (cachable.IsDirty || !_imageBuffer.ContainsKey(renderer))
                    {
                        _renderCacheDrawTimes[renderer].Start();

                        using IImageCanvas? imageCanvas = _imageFactory.CreateImageCanvas(_width, _height);
                        renderer.Render(imageCanvas.Canvas, _width, _height);
                        _imageBuffer[renderer] = imageCanvas.Render();
                        _renderCacheDrawTimes[renderer].Stop();
                    }

                    _renderLayerDrawTimes[renderer].Start();
                    canvas.DrawImage(_imageBuffer[renderer], 0, 0);
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

        public void Dispose()
        {
            _backBuffer?.Dispose();
            _renderLoop.Dispose();
            _gameBoard.Dispose();
        }
    }
}
