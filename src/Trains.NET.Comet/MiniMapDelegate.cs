using System.Drawing;
using Comet.Skia;
using SkiaSharp;
using Trains.NET.Engine;
using Trains.NET.Rendering;
using Trains.NET.Rendering.Skia;

namespace Trains.NET.Comet
{
    internal class MiniMapDelegate : AbstractControlDelegate
    {
        private readonly IGameBoard _gameBoard;
        private readonly ITrackRenderer _trackRenderer;
        private readonly IPixelMapper _pixelMapper;
        private readonly ITrackParameters _trackParameters;
        private readonly TrackLayoutRenderer _renderer;

        public MiniMapDelegate(IGameBoard gameBoard, ITrackRenderer trackRenderer, ITrackParameters trackParameters)
        {
            _gameBoard = gameBoard;
            _trackRenderer = trackRenderer;
            _trackParameters = trackParameters;

            _pixelMapper = new PixelMapper(_trackParameters);

            _renderer = new TrackLayoutRenderer(_gameBoard, _trackRenderer, _pixelMapper, _trackParameters);
        }

        public override void Draw(SKCanvas canvas, RectangleF dirtyRect)
        {
            const int maxGridSize = PixelMapper.MaxGridSize;
            using var bitmap = new SKBitmap(maxGridSize, maxGridSize);

            using var tempCanvas = new SKCanvas(bitmap);
            tempCanvas.Clear(SKColors.White);
            using var canvasWrapper = new SKCanvasWrapper(tempCanvas);
            int oldPadding = _trackParameters.PlankPadding;
            int oldWidth = _trackParameters.PlankWidth;
            _trackParameters.PlankPadding = 1;
            _trackParameters.PlankWidth = 20;
            _renderer.Render(canvasWrapper, maxGridSize, maxGridSize);
            _trackParameters.PlankPadding = oldPadding;
            _trackParameters.PlankWidth = oldWidth;
            canvas.DrawBitmap(bitmap, new SKRect(0, 0, maxGridSize, maxGridSize), new SKRect(0, 0, 100, 100));
        }
    }
}
