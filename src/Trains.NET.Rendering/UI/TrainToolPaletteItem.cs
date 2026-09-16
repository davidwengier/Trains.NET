using Trains.NET.Engine;

namespace Trains.NET.Rendering.UI;

[Order(50)]
public class TrainToolPaletteItem(TrainTool tool, IGameManager gameManager, IRenderer<Train> renderer) : IToolPaletteItem
{
    public ITool Tool { get; } = tool;

    public ButtonBase Button { get; } =
        new RendererButton<Train>(
            new Train(0),
            renderer,
            () => gameManager.CurrentTool == tool,
            () => gameManager.CurrentTool = tool)
        {
            Height = 40
        };
}
