using Trains.NET.Engine;

namespace Trains.NET.Rendering.UI;

[Order(15)]
public class TreeToolPaletteItem(TreeTool tool, IGameManager gameManager, IStaticEntityRenderer<Tree> renderer) : IToolPaletteItem
{
    public ITool Tool { get; } = tool;

    public ButtonBase Button { get; } =
        new RendererButton<Tree>(
            new Tree(1),
            renderer,
            () => gameManager.CurrentTool == tool,
            () => gameManager.CurrentTool = tool)
        {
            Height = 40
        };
}
