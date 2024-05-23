using Trains.NET.Engine;

namespace Trains.NET.Rendering.UI;

public class BuildModeButton(IGameManager gameManager) : MultiButton(34, GetButtons(gameManager))
{
    private static ButtonBase[] GetButtons(IGameManager gameManager)
    {
        return [
            new PictureButton(Picture.Tools, 20, () => gameManager.BuildMode, () => gameManager.BuildMode = true),
            new PictureButton(Picture.Play, 20, () => !gameManager.BuildMode, () => gameManager.BuildMode = false)
        ];
    }
}
