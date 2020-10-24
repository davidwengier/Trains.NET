using Trains.NET.Engine;

namespace Trains.NET.Rendering.UI
{
    public class BuildModeButton : MultiButton
    {
        public BuildModeButton(Engine.IGameManager gameManager)
            : base(34, GetButtons(gameManager))
        {
        }

        private static Button[] GetButtons(IGameManager gameManager)
        {
            return new Button[]{
                new Button("{{fa-wrench}}", () => gameManager.BuildMode, () => gameManager.BuildMode = true)
                {
                    LabelBrush = Brushes.Label with { TextSize = 20 }
                },
                new Button("{{fa-play}}", () => !gameManager.BuildMode, () => gameManager.BuildMode = false)
                {
                    LabelBrush = Brushes.Label with { TextSize = 20 }
                }
                };
        }
    }
}
