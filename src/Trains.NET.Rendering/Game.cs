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
        private readonly ElapsedMillisecondsTimedStat _skiaDrawTime = InstrumentationBag.Add<ElapsedMillisecondsTimedStat>("Draw-AllUp-Skia");
        private readonly Dictionary<ILayerRenderer, ElapsedMillisecondsTimedStat> _renderLayerDrawTimes;
        private readonly Dictionary<ILayerRenderer, IBitmap> _bitmapBuffer = new Dictionary<ILayerRenderer, IBitmap>();

        public Game(IGameBoard gameBoard, OrderedList<ILayerRenderer> boardRenderers, IPixelMapper pixelMapper, IBitmapFactory bitmapFactory)
        {
            _gameBoard = gameBoard;
            _boardRenderers = boardRenderers;
            _pixelMapper = pixelMapper;
            _bitmapFactory = bitmapFactory;
            _renderLayerDrawTimes = _boardRenderers.ToDictionary(x => x, x => InstrumentationBag.Add<ElapsedMillisecondsTimedStat>(GetLayerDiagnosticsName(x)));
            _pixelMapper.ViewPortChanged += (s, e) => _needsBufferReset = true;
        }

        private static string GetLayerDiagnosticsName(ILayerRenderer layerRenderer)
        {
            var sb = new StringBuilder("Draw-Layer-");
            sb.Append(layerRenderer.Name.Replace(" ", ""));
            if(layerRenderer is ICachableLayerRenderer)
            {
                sb.Append("[Cachable]");
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
            canvas.Clear(Colors.VeryLightGray);

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
                        if (!_bitmapBuffer.TryGetValue(renderer, out IBitmap bitmap))
                        {
                            bitmap = _bitmapFactory.CreateBitmap(_width, _height);
                        }
                        ICanvas layerCanvas = _bitmapFactory.CreateCanvas(bitmap);
                        layerCanvas.Clear(Colors.Empty);
                        RenderLayer(renderer, layerCanvas);
                        _bitmapBuffer[renderer] = bitmap;
                        layerCanvas.Dispose();
                    }

                    canvas.DrawBitmap(_bitmapBuffer[renderer], 0, 0);
                }
                else
                {
                    RenderLayer(renderer, canvas);
                }
                canvas.Restore();
            }
            canvas.Restore();

            if (_needsBufferReset)
            {
                ResetBuffers();
                _needsBufferReset = false;
            }

            _skiaDrawTime.Stop();
            _skiaFps.Update();

            void RenderLayer(ILayerRenderer renderer, ICanvas layerCanvas)
            {
                _renderLayerDrawTimes[renderer].Start();
                renderer.Render(layerCanvas, _width, _height);
                _renderLayerDrawTimes[renderer].Stop();
            }
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
