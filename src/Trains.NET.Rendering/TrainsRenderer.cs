using SkiaSharp;
using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    internal class TrainsRenderer : ILayerRenderer
    {
        private readonly IGameBoard _gameBoard;
        private readonly ITrainRenderer _trainRenderer;
        private readonly ITrackParameters _trackParameters;
        private readonly IPixelMapper _pixelMapper;

        public bool Enabled { get; set; } = true;

        public string Name => "Trains";

        public TrainsRenderer(IGameBoard gameBoard, ITrainRenderer trainRenderer, ITrackParameters trackParameters, IPixelMapper pixelMapper)
        {
            _gameBoard = gameBoard;
            _trainRenderer = trainRenderer;
            _trackParameters = trackParameters;
            _pixelMapper = pixelMapper;
        }

        public void Render(SKCanvas canvas, int width, int height)
        {
            foreach (Train train in _gameBoard.GetTrains())
            {
                canvas.Save();

                (int x, int y) = _pixelMapper.CoordsToPixels(train.Column, train.Row);

                canvas.Translate(x, y);

                _trainRenderer.Render(canvas, train);

                canvas.Restore();
            }
        }
    }
}
