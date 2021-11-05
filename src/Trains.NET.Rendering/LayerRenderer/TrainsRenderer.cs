using Trains.NET.Engine;

namespace Trains.NET.Rendering;

[Order(500)]
public class TrainsRenderer : ILayerRenderer
{
    private readonly IMovableLayout _movableLayout;
    private readonly ILayout _layout;
    private readonly IRenderer<Train> _trainRenderer;
    private readonly CarriageRenderer _carriageRenderer;

    public bool Enabled { get; set; } = true;

    public string Name => "Trains";

    public TrainsRenderer(IMovableLayout movableLayout, ILayout layout, IRenderer<Train> trainRenderer, CarriageRenderer carriageRenderer)
    {
        _movableLayout = movableLayout;
        _layout = layout;
        _trainRenderer = trainRenderer;
        _carriageRenderer = carriageRenderer;
    }

    public void Render(ICanvas canvas, int width, int height, IPixelMapper pixelMapper)
    {
        foreach (Train train in _movableLayout.Get())
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

                var steps = TrainMovement.GetNextSteps(_layout, fakeTrain, 1.0f);
                foreach (var step in steps)
                {
                    fakeTrain.ApplyStep(step);
                }
            }
        }
    }
}
