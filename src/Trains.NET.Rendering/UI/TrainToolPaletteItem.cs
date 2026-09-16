using Trains.NET.Engine;

namespace Trains.NET.Rendering.UI;

[Order(50)]
public class TrainToolPaletteItem(
    TrainTool tool,
    IGameManager gameManager,
    IRenderer<Train> renderer,
    ITooltipService tooltipService) : IToolPaletteItem
{
    public ITool Tool { get; } = tool;

    public ButtonBase Button { get; } =
        new RendererButton<Train>(
            new Train(0),
            renderer,
            () => gameManager.CurrentTool == tool,
            () => gameManager.CurrentTool = tool,
            tooltipService: tooltipService,
            tooltip: tool.Name)
        {
            Height = 40
        };
}
