using SkiaSharp;
using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
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

        public void Render(SKCanvas canvas, int width, int height)
        {
            foreach (Train train in _gameBoard.GetTrains())
            {
                canvas.Save();
                _trainRenderer.Render(canvas, train);
            }
        }
    }
}
