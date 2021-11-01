using Trains.NET.Engine;

namespace Trains.NET.Rendering;

[Order(500)]
public class TrainsRenderer : ILayerRenderer
{
    private readonly IGameBoard _gameBoard;
    private readonly IRenderer<Train> _trainRenderer;
    private readonly CarriageRenderer _carriageRenderer;

    public bool Enabled { get; set; } = true;

    public string Name => "Trains";

    public TrainsRenderer(IGameBoard gameBoard, IRenderer<Train> trainRenderer, CarriageRenderer carriageRenderer)
    {
        _gameBoard = gameBoard;
        _trainRenderer = trainRenderer;
        _carriageRenderer = carriageRenderer;
    }

    public void Render(ICanvas canvas, int width, int height, IPixelMapper pixelMapper)
    {
        foreach (Train train in _gameBoard.GetMovables())
        {
            // Create a fake train pointing backwards, to represent our carriage
            var fakeTrain = train.Clone();
            for (var i = 0; i <= train.Carriages; i++)
            {
                using (canvas.Scope())
                {
                    (int x, int y, bool onScreen) = pixelMapper.CoordsToViewPortPixels(fakeTrain.Column, fakeTrain.Row);

                    if (onScreen)
                    {
                        canvas.Translate(x, y);

                        float scale = pixelMapper.CellSize / 100.0f;

                        canvas.Scale(scale, scale);

                        if (i == train.Carriages)
                        {
                            _trainRenderer.Render(canvas, fakeTrain);
                        }
                        else
                        {
                            _carriageRenderer.Render(canvas, fakeTrain);
                        }
                    }
                }

                var steps = _gameBoard.GetNextSteps(fakeTrain, 1.0f);
                foreach (var step in steps)
                {
                    fakeTrain.ApplyStep(step);
                }
            }
        }
    }
}
