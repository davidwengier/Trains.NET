using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    [Order(500)]
    public class TrainsRenderer : ILayerRenderer
    {
        private readonly IGameBoard _gameBoard;
        private readonly IRenderer<Train> _trainRenderer;

        public bool Enabled { get; set; } = true;

        public string Name => "Trains";

        public TrainsRenderer(IGameBoard gameBoard, IRenderer<Train> trainRenderer)
        {
            _gameBoard = gameBoard;
            _trainRenderer = trainRenderer;
        }

        public void Render(ICanvas canvas, int width, int height, IPixelMapper pixelMapper)
        {
            foreach (Train train in _gameBoard.GetMovables())
            {
                canvas.Save();

                (int x, int y, bool onScreen) = pixelMapper.CoordsToViewPortPixels(train.Column, train.Row);

                if (!onScreen) continue;

                canvas.Translate(x, y);

                float scale = pixelMapper.CellSize / 100.0f;

                canvas.Scale(scale, scale);

                _trainRenderer.Render(canvas, train);

                canvas.Restore();
            }
        }
    }
}
