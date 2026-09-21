using Trains.NET.Engine;

namespace Trains.NET.Rendering.UI;

[Order(17)]
public class DepotToolButton(
    DepotTool tool,
    IGameManager gameManager,
    IStaticEntityRenderer<Depot> renderer,
    ITooltipService tooltipService) : IToolButton
{
    public ButtonBase Button { get; } =
        new RendererButton<Depot>(
            Depot.CreateNew(0),
            renderer,
            () => gameManager.CurrentTool == tool,
            () => gameManager.CurrentTool = tool,
            tooltipService: tooltipService,
            tooltip: tool.Name)
        {
            Height = 40
        };
}
