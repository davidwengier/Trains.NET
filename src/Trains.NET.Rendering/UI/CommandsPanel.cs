using System.Collections.Generic;
using System.Linq;
using Trains.NET.Engine;

namespace Trains.NET.Rendering.UI
{
    public class CommandsPanel : ButtonPanelBase
    {
        private readonly List<Button> _buttons;
        private readonly IGameManager _gameManager;

        protected override int Top => 250;

        public CommandsPanel(IEnumerable<ICommand> commands, IGameManager gameManager)
        {
            _buttons = commands.Select(c => new Button(c.Name, c, () => false, () => c.Execute())).ToList();
            _gameManager = gameManager;

            _gameManager.Changed += (s, e) => OnChanged();
        }

        protected override IEnumerable<Button> GetButtons()
            => _buttons;

        public override void Render(ICanvas canvas, int width, int height)
        {
            if (_gameManager.BuildMode)
            {
                base.Render(canvas, width, height);
            }
        }
    }
}
