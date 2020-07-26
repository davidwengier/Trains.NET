using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    [Order(500)]
    internal class TrainsRenderer : ILayerRenderer
    {
        private readonly IGameBoard _gameBoard;
        private readonly ITrainRenderer _trainRenderer;

        public bool Enabled { get; set; } = true;

        public string Name => "Trains";

        public TrainsRenderer(IGameBoard gameBoard, ITrainRenderer trainRenderer)
        {
            _gameBoard = gameBoard;
            _trainRenderer = trainRenderer;
        }

        public void Render(ICanvas canvas, int width, int height, IPixelMapper pixelMapper)
        {
            foreach (Train train in _gameBoard.GetMovables())
            {
                canvas.Save();

                (int x, int y) = pixelMapper.CoordsToViewPortPixels(train.Column, train.Row);

                canvas.Translate(x, y);

                _trainRenderer.Render(canvas, train);

                canvas.Restore();
            }
        }
    }
}
