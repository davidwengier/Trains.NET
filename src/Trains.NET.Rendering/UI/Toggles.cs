using System.Collections.Generic;
using System.Linq;
using Trains.NET.Engine;

namespace Trains.NET.Rendering.UI
{
    public class Toggles : ButtonPanelBase
    {
        private readonly Button[] _buttons;

        protected override bool Collapsed { get; set; } = true;
        protected override string? Title => "Configuration";
        protected override int Top => 200;
        protected override PanelSide Side => PanelSide.Right;

        public Toggles(IEnumerable<ITogglable> togglables)
        {
            _buttons = togglables.Select(t => new Button(t.Name, t, () => t.Enabled, () => t.Enabled = !t.Enabled)).ToArray();
        }

        protected override IEnumerable<Button> GetButtons()
            => _buttons;
    }
}
