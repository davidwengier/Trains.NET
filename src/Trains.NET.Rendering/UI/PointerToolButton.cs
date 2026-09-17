using Trains.NET.Engine;

namespace Trains.NET.Rendering.UI;

[Order(2)]
public class PointerToolButton(PointerTool tool, IGameManager gameManager, ITooltipService tooltipService) : IToolButton
{
    public ButtonBase Button { get; } =
        new PictureButton(
            Picture.Pointer,
            32,
            () => gameManager.CurrentTool == tool,
            () => gameManager.CurrentTool = tool,
            tooltipService: tooltipService,
            tooltip: tool.Name)
        {
            Width = 40,
            Height = 40
        };
}
