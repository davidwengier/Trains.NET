using System;
using Trains.NET.Engine;

namespace Trains.NET.Rendering.UI
{
    public class BuildModeButton : MultiButton
    {
        private readonly IGameManager _gameManager;

        public BuildModeButton(Engine.IGameManager gameManager)
            : base(20, 34, GetButtons(gameManager))
        {
            _gameManager = gameManager;
        }

        private static Button[] GetButtons(IGameManager gameManager)
        {
            return new Button[]{
                new Button("{{fa-wrench}}", () => gameManager.BuildMode, () => gameManager.BuildMode = true),
                new Button("{{fa-play}}", () => !gameManager.BuildMode, () => gameManager.BuildMode = false)
                };
        }
    }
}
