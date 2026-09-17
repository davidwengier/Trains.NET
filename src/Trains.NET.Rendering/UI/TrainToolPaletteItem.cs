using Trains.NET.Engine;

namespace Trains.NET.Rendering.UI;

[Order(50)]
public class TrainToolPaletteItem(
    TrainTool tool,
    IGameManager gameManager,
    IRenderer<Train> renderer,
    ITooltipService tooltipService) : IToolPaletteItem
{
    // Chosen to produce a light blue palette.
    private const int LightBlueTrainSeed = 39477;

    public ButtonBase Button { get; } =
        new RendererButton<Train>(
            new Train(LightBlueTrainSeed),
            renderer,
            () => gameManager.CurrentTool == tool,
            () => gameManager.CurrentTool = tool,
            tooltipService: tooltipService,
            tooltip: tool.Name)
        {
            Height = 40
        };
}
