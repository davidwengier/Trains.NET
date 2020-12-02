using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    [Order(500)]
    public class TrainsRenderer : ILayerRenderer
    {
        private readonly IGameBoard _gameBoard;
        private readonly IRenderer<Train> _trainRenderer;
        private readonly IRenderer<Carriage> _carriageRenderer;

        public bool Enabled { get; set; } = true;

        public string Name => "Trains";

        public TrainsRenderer(IGameBoard gameBoard, IRenderer<Train> trainRenderer, IRenderer<Carriage> carriageRenderer)
        {
            _gameBoard = gameBoard;
            _trainRenderer = trainRenderer;
            _carriageRenderer = carriageRenderer;
        }

        public void Render(ICanvas canvas, int width, int height, IPixelMapper pixelMapper)
        {
            foreach (Train train in _gameBoard.GetMovables())
            {
                using (canvas.Scope())
                {
                    (int x, int y, bool onScreen) = pixelMapper.CoordsToViewPortPixels(train.Column, train.Row);

                    if (!onScreen) continue;

                    canvas.Translate(x, y);

                    float scale = pixelMapper.CellSize / 100.0f;

                    canvas.Scale(scale, scale);

                    _trainRenderer.Render(canvas, train);

                }

                foreach (var carriage in train.GetCarriages())
                {
                    using (canvas.Scope())
                    {
                        (int x, int y, bool onScreen) = pixelMapper.CoordsToViewPortPixels(carriage.Column, carriage.Row);

                        if (!onScreen) continue;

                        canvas.Translate(x, y);

                        float scale = pixelMapper.CellSize / 100.0f;

                        canvas.Scale(scale, scale);

                        _carriageRenderer.Render(canvas, carriage);
                    }
                }
            }
        }
    }
}
