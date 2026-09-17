using Trains.NET.Engine;

namespace Trains.NET.Rendering.UI;

[Order(20)]
public class EraserToolButton(EraserTool tool, IGameManager gameManager, ITooltipService tooltipService) : IToolButton
{
    public ButtonBase Button { get; } =
        new PictureButton(
            Picture.Eraser,
            20,
            () => gameManager.CurrentTool == tool,
            () => gameManager.CurrentTool = tool,
            tooltipService: tooltipService,
            tooltip: tool.Name)
        {
            Width = 40,
            Height = 40
        };
}
