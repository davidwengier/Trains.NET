using System.Collections.Generic;
using System.Linq;
using Trains.NET.Engine;

namespace Trains.NET.Rendering.UI;

[Order(150)]
public class Toggles : ButtonPanelBase
{
    private readonly TextButton[] _buttons;

    protected override string? Title => "Configuration";
    protected override bool IsCollapsable => true;
    protected override int Top => 200;
    protected override PanelPosition Position => PanelPosition.Right;

    public Toggles(IEnumerable<ITogglable> togglables)
    {
        _buttons = togglables.Select(t => new TextButton(t.Name, () => t.Enabled, () => t.Enabled = !t.Enabled)).ToArray();
    }

    protected override IEnumerable<TextButton> GetButtons()
        => _buttons;
}
