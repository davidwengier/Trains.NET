using Trains.NET.Engine;

namespace Trains.NET.Rendering.UI;

[Order(0)]
public class PlaybackToolButton(
    IGameManager gameManager,
    ITooltipService tooltipService) : IToolButton
{
    public ButtonBase Button { get; } =
        new MultiButton(
            40,
            new PictureButton(
                Picture.Play,
                20,
                () => !gameManager.Paused,
                () => gameManager.Paused = false,
                tooltipService,
                "Play"),
            new PictureButton(
                Picture.Pause,
                20,
                () => gameManager.Paused,
                () => gameManager.Paused = true,
                tooltipService,
                "Pause"));
}
