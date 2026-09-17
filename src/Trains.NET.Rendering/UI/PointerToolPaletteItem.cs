using Trains.NET.Engine;

namespace Trains.NET.Rendering.UI;

[Order(2)]
public class PointerToolPaletteItem(PointerTool tool, IGameManager gameManager, ITooltipService tooltipService) : IToolPaletteItem
{
    public ITool Tool { get; } = tool;

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
