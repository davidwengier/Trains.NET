using Trains.NET.Engine;

namespace Trains.NET.Rendering.UI;

public class CommandsPanel(IEnumerable<ICommand> commands) : ButtonPanelBase
{
    private readonly List<TextButton> _buttons = commands.Select(c => new TextButton(c.Name, () => false, () => c.Execute())).ToList();

    protected override bool IsCollapsable => true;
    protected override string? Title => "Commands";
    protected override int Top => 250;

    protected override IEnumerable<TextButton> GetButtons()
        => _buttons;
}
