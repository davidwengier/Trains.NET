using System.Collections.Generic;
using System.Linq;
using Trains.NET.Engine;

namespace Trains.NET.Rendering.UI
{
    public class CommandsPanel : ButtonPanelBase
    {
        private readonly List<Button> _buttons;

        protected override bool IsCollapsable => true;
        protected override string? Title => "Commands";
        protected override int Top => 250;
#if DEBUG
        protected override PanelPosition Position => PanelPosition.Floating;
        protected override int Left => 200;
#endif

        public CommandsPanel(IEnumerable<ICommand> commands)
        {
            _buttons = commands.Select(c => new Button(c.Name, () => false, () => c.Execute())).ToList();
        }

        protected override IEnumerable<Button> GetButtons()
            => _buttons;
    }
}
