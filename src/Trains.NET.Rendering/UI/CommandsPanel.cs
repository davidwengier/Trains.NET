using Trains.NET.Engine;

namespace Trains.NET.Rendering.UI;

public class CommandsPanel : ButtonPanelBase
{
    private readonly List<TextButton> _buttons;

    protected override bool IsCollapsable => true;
    protected override string? Title => "Commands";
    protected override int Top => 250;

    public CommandsPanel(IEnumerable<ICommand> commands)
    {
        _buttons = commands.Select(c => new TextButton(c.Name, () => false, () => c.Execute())).ToList();
    }

    protected override IEnumerable<TextButton> GetButtons()
        => _buttons;
}
