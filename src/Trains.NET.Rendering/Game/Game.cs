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

        public Game(IGameBoard gameBoard,
                    IEnumerable<ILayerRenderer> boardRenderers,
                    IPixelMapper pixelMapper,
                    IImageFactory imageFactory,
                    ITimer renderLoop,
                    IEnumerable<IScreen> screens,
                    IImageCache imageCache)
        {
            _gameBoard = gameBoard;
            _boardRenderers = boardRenderers;
            _pixelMapper = pixelMapper;
            _imageFactory = imageFactory;
            _renderLoop = renderLoop;
            _screens = screens;
            _imageCache = imageCache;

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

        public void Render(ICanvas canvas)
        {
            if (_width == 0 || _height == 0)
            {
                return;
            }

            IImage? gameImage = _imageCache.Get(this);
            if (gameImage != null)
            {
                canvas.DrawImage(gameImage, 0, 0);
            }

            foreach (IScreen screen in _screens)
            {
                IImage? screenImage = _imageCache.Get(screen);
                if (screenImage != null)
                {
                    canvas.DrawImage(screenImage, 0, 0);
                }
            }
        }

        public void DrawFrame()
        {
            if (_width == 0 || _height == 0) return;

            AdjustViewPortIfNecessary();

            using (_ = _imageCache.SuspendSetDirtyCalls())
            {
                IPixelMapper pixelMapper = _pixelMapper.Snapshot();

                using IImageCanvas imageCanvas = _imageFactory.CreateImageCanvas(_width, _height);

                RenderFrame(imageCanvas.Canvas, pixelMapper);

                _imageCache.Set(this, imageCanvas.Render());

                foreach (IScreen screen in _screens)
                {
                    if (_imageCache.IsDirty(screen))
                    {
                        _screenDrawTimes[screen].Start();
                        using IImageCanvas screnCanvas = _imageFactory.CreateImageCanvas(_screenWidth, _screenHeight);
                        screen.Render(screnCanvas.Canvas, _screenWidth, _screenHeight);
                        _screenDrawTimes[screen].Stop();
                        _imageCache.Set(screen, screnCanvas.Render());
                    }
                }
            }
        }

        private void RenderFrame(ICanvas canvas, IPixelMapper pixelMapper)
        {
            _skiaDrawTime.Start();

            canvas.Save();

            foreach (ILayerRenderer renderer in _boardRenderers)
            {
                if (!renderer.Enabled)
                {
                    continue;
                }
                canvas.Save();

                if (renderer is ICachableLayerRenderer cachable)
                {
                    if (_imageCache.IsDirty(renderer))
                    {
                        _renderCacheDrawTimes[renderer].Start();

                        using IImageCanvas imageCanvas = _imageFactory.CreateImageCanvas(_width, _height);
                        renderer.Render(imageCanvas.Canvas, _width, _height, pixelMapper);
                        _imageCache.Set(renderer, imageCanvas.Render());
                        _renderCacheDrawTimes[renderer].Stop();
                    }

                    _renderLayerDrawTimes[renderer].Start();
                    canvas.DrawImage(_imageCache.Get(renderer)!, 0, 0);
                    _renderLayerDrawTimes[renderer].Stop();
                }
                else
                {
                    _renderLayerDrawTimes[renderer].Start();
                    renderer.Render(canvas, _width, _height, pixelMapper);
                    _renderLayerDrawTimes[renderer].Stop();
                }
                canvas.Restore();
            }
            canvas.Restore();

            _skiaDrawTime.Stop();
            _skiaFps.Update();
        }

        public void AdjustViewPortIfNecessary()
        {
            foreach (IMovable vehicle in _gameBoard.GetMovables())
            {
                if (vehicle.Follow)
                {
                    (int x, int y, _) = _pixelMapper.CoordsToViewPortPixels(vehicle.Column, vehicle.Row);

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
            _renderLoop.Dispose();
            _imageCache.Dispose();
            _gameBoard.Dispose();
        }
    }
}
