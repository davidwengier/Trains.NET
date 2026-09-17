using Trains.NET.Engine;

namespace Trains.NET.Rendering.UI;

[Order(15)]
public class TreeToolButton(
    TreeTool tool,
    IGameManager gameManager,
    IStaticEntityRenderer<Tree> renderer,
    ITooltipService tooltipService) : IToolButton
{
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
