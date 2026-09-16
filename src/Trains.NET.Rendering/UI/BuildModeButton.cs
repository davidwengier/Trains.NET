using Trains.NET.Engine;

namespace Trains.NET.Rendering.UI;

public class BuildModeButton(IGameManager gameManager, ITooltipService tooltipService) : MultiButton(34, GetButtons(gameManager, tooltipService))
{
    private static ButtonBase[] GetButtons(IGameManager gameManager, ITooltipService tooltipService)
    {
        return [
            new PictureButton(Picture.Tools, 20, () => gameManager.BuildMode, () => gameManager.BuildMode = true, tooltipService: tooltipService, tooltip: "Build mode"),
            new PictureButton(Picture.Play, 20, () => !gameManager.BuildMode, () => gameManager.BuildMode = false, tooltipService: tooltipService, tooltip: "Play mode")
        ];
    }
}
