using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trains.NET.Engine;
using Trains.NET.Instrumentation;

namespace Trains.NET.Rendering
{
    public class Game : IGame
    {
        private const int RenderInterval = 16;

        private int _width;
        private int _height;
        private int _screenWidth;
        private int _screenHeight;
        private readonly IGameBoard _gameBoard;
        private readonly IEnumerable<ILayerRenderer> _boardRenderers;
        private readonly IPixelMapper _pixelMapper;
        private readonly IImageFactory _imageFactory;
        private readonly PerSecondTimedStat _skiaFps = InstrumentationBag.Add<PerSecondTimedStat>("Draw-FPS-Skia");
        private readonly ElapsedMillisecondsTimedStat _skiaDrawTime = InstrumentationBag.Add<ElapsedMillisecondsTimedStat>("Draw-Skia-AllUp");
        private readonly Dictionary<ILayerRenderer, ElapsedMillisecondsTimedStat> _renderLayerDrawTimes;
        private readonly Dictionary<IScreen, ElapsedMillisecondsTimedStat> _screenDrawTimes;
        private readonly Dictionary<ILayerRenderer, ElapsedMillisecondsTimedStat> _renderCacheDrawTimes;
        private readonly ITimer _renderLoop;
        private readonly IEnumerable<IScreen> _screens;
        private readonly IImageCache _imageCache;
        private readonly ISwapChain _swapChain;

        public Game(IGameBoard gameBoard,
                    IEnumerable<ILayerRenderer> boardRenderers,
                    IPixelMapper pixelMapper,
                    IImageFactory imageFactory,
                    ITimer renderLoop,
                    IEnumerable<IScreen> screens,
                    IImageCache imageCache,
                    ISwapChain swapChain)
        {
            _gameBoard = gameBoard;
            _boardRenderers = boardRenderers;
            _pixelMapper = pixelMapper;
            _imageFactory = imageFactory;
            _renderLoop = renderLoop;
            _screens = screens;
            _imageCache = imageCache;
            _swapChain = swapChain;

            foreach (IScreen screen in _screens)
            {
                screen.Changed += (s, e) => _imageCache.SetDirty(screen);
            }
            foreach (ICachableLayerRenderer renderer in _boardRenderers.OfType<ICachableLayerRenderer>())
            {
                renderer.Changed += (s, e) => _imageCache.SetDirty(renderer);
            }

            _renderLayerDrawTimes = _boardRenderers.ToDictionary(x => x, x => InstrumentationBag.Add<ElapsedMillisecondsTimedStat>(GetLayerDiagnosticsName(x)));
            _screenDrawTimes = _screens.ToDictionary(x => x, x => InstrumentationBag.Add<ElapsedMillisecondsTimedStat>(GetLayerDiagnosticsName(x)));
            _renderCacheDrawTimes = _boardRenderers.Where(x => x is ICachableLayerRenderer).ToDictionary(x => x, x => InstrumentationBag.Add<ElapsedMillisecondsTimedStat>("Draw-Cache-" + x.Name.Replace(" ", "")));
            _pixelMapper.ViewPortChanged += (s, e) => _imageCache.SetDirtyAll(_boardRenderers);

            _renderLoop.Elapsed += (s, e) => DrawFrame();
            _renderLoop.Interval = RenderInterval;
            _renderLoop.Start();

            _gameBoard.Initialize(_pixelMapper.Columns, _pixelMapper.Rows);
        }

        private static string GetLayerDiagnosticsName(ILayerRenderer layerRenderer)
        {
            var sb = new StringBuilder("Draw-Layer-");
            sb.Append(layerRenderer.Name.Replace(" ", ""));
            if (layerRenderer is ICachableLayerRenderer)
            {
                sb.Append("[Cached]");
            }
            return sb.ToString();
        }
        private static string GetLayerDiagnosticsName(IScreen screen)
        {
            var sb = new StringBuilder("Draw-Screen-");
            sb.Append(screen.GetType().Name.Replace(" ", ""));
            return sb.ToString();
        }

        public void SetSize(int width, int height)
        {
            _screenWidth = width;
            _screenHeight = height;

            _imageCache.SetDirtyAll(_screens);

            (int columns, int rows) = _pixelMapper.ViewPortPixelsToCoords(width, height);
            columns = Math.Max(columns, 1);
            rows = Math.Max(rows, 1);

            (width, height, _) = _pixelMapper.CoordsToViewPortPixels(columns + 1, rows + 1);

            if (_width != width || _height != height)
            {
                _width = width;
                _height = height;
                _pixelMapper.SetViewPortSize(_width, _height);

                // strictly speaking this only needs to clear renderers, but we already cleared screens
                // so its easier just to call clear
                _imageCache.Clear();
            }
        }

        public (int Width, int Height) GetSize() => (_width, _height);

        public (int Width, int Height) GetScreenSize() => (_screenWidth, _screenHeight);

        public void Render(Action<ISwapChain> render)
        {
            if (_width == 0 || _height == 0)
            {
                return;
            }

            render(_swapChain);
        }

        public void DrawFrame()
        {
            if (_width == 0 || _height == 0) return;

            AdjustViewPortIfNecessary();

            using (_imageCache.SuspendSetDirtyCalls())
            {
                IPixelMapper pixelMapper = _pixelMapper.Snapshot();

                _swapChain.DrawNext(canvas =>
                {
                    canvas.Clear(Colors.White);

                    DrawFrame(canvas, pixelMapper);
                });
            }
        }

        private void DrawFrame(ICanvas canvas, IPixelMapper pixelMapper)
        {
            using (canvas.Scope())
            {
                RenderFrame(canvas, pixelMapper);
            }

            foreach (IScreen screen in _screens)
            {
                using (_screenDrawTimes[screen].Measure())
                using (canvas.Scope())
                {
                    screen.Render(canvas, _screenWidth, _screenHeight);
                }
            }
        }

        private void RenderFrame(ICanvas canvas, IPixelMapper pixelMapper)
        {
            using (_skiaDrawTime.Measure())
            using (canvas.Scope())
            {
                foreach (ILayerRenderer layerRenderer in _boardRenderers)
                {
                    if (!layerRenderer.Enabled)
                    {
                        continue;
                    }

                    RenderLayer(canvas, pixelMapper, layerRenderer);
                }
            }

            _skiaFps.Update();
        }

        private void RenderLayer(ICanvas canvas, IPixelMapper pixelMapper, ILayerRenderer layerRenderer)
        {
            using (canvas.Scope())
            {
                if (layerRenderer is ICachableLayerRenderer cachable)
                {
                    RenderCachedLayer(canvas, pixelMapper, layerRenderer);
                }
                else
                {
                    using (_renderLayerDrawTimes[layerRenderer].Measure())
                    {
                        layerRenderer.Render(canvas, _width, _height, pixelMapper);
                    }
                }
            }
        }

        private void RenderCachedLayer(ICanvas canvas, IPixelMapper pixelMapper, ILayerRenderer layerRenderer)
        {
            if (_imageCache.IsDirty(layerRenderer))
            {
                using (_renderCacheDrawTimes[layerRenderer].Measure())
                {
                    using IImageCanvas imageCanvas = _imageFactory.CreateImageCanvas(_width, _height);
                    layerRenderer.Render(imageCanvas.Canvas, _width, _height, pixelMapper);
                    _imageCache.Set(layerRenderer, imageCanvas.Render());
                }
            }

            using (_renderLayerDrawTimes[layerRenderer].Measure())
            {
                canvas.DrawImage(_imageCache.Get(layerRenderer)!, 0, 0);
            }
        }

        public void AdjustViewPortIfNecessary()
        {
            foreach (IMovable vehicle in _gameBoard.GetMovables())
            {
                if (vehicle.Follow)
                {
                    (int x, int y) = _pixelMapper.CoordsToWorldPixels(vehicle.Column, vehicle.Row);

                    x += (int)(_pixelMapper.CellSize * vehicle.RelativeLeft);
                    y += (int)(_pixelMapper.CellSize * vehicle.RelativeTop);

                    _pixelMapper.SetViewPortCenter(x, y);

                    break;
                }
            }
        }

        public void Dispose()
        {
            _renderLoop.Dispose();
            _imageCache.Dispose();
            _gameBoard.Dispose();
        }
    }
}
