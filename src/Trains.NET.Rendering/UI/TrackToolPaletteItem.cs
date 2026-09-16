using Trains.NET.Engine;

namespace Trains.NET.Rendering.UI;

[Order(10)]
public class TrackToolPaletteItem(
    TrackTool tool,
    IGameManager gameManager,
    IEnumerable<IStaticEntityRenderer<Track>> renderers) : IToolPaletteItem
{
    public ITool Tool { get; } = tool;

    public ButtonBase Button { get; } =
        new TrackButton(
            new SingleTrack { Direction = SingleTrackDirection.Horizontal },
            () => gameManager.CurrentTool == tool,
            () => gameManager.CurrentTool = tool,
            renderers)
        {
            Height = 40
        };
}
