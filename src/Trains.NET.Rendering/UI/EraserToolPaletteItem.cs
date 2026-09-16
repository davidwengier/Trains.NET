using Trains.NET.Engine;

namespace Trains.NET.Rendering.UI;

[Order(20)]
public class EraserToolPaletteItem(EraserTool tool, IGameManager gameManager) : IToolPaletteItem
{
    public ITool Tool { get; } = tool;

    public ButtonBase Button { get; } =
        new PictureButton(Picture.Eraser, 20, () => gameManager.CurrentTool == tool, () => gameManager.CurrentTool = tool)
        {
            Width = 40,
            Height = 40
        };
}
