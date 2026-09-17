using Trains.NET.Engine;

namespace Trains.NET.Rendering.UI;

[Order(15)]
public class TreeToolPaletteItem(
    TreeTool tool,
    IGameManager gameManager,
    IStaticEntityRenderer<Tree> renderer,
    ITooltipService tooltipService) : IToolPaletteItem
{
    public ITool Tool { get; } = tool;

    public ButtonBase Button { get; } =
        new RendererButton<Tree>(
            new Tree(1),
            renderer,
            () => gameManager.CurrentTool == tool,
            () => gameManager.CurrentTool = tool,
            tooltipService: tooltipService,
            tooltip: tool.Name)
        {
            Height = 40
        };
}
