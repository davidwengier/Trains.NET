using Trains.NET.Engine;

namespace Trains.NET.Rendering.UI;

[Order(100)]
public class ToolsPanel : ButtonPanelBase
{
    private readonly IEnumerable<ButtonBase> _buttons;

    protected override int Top => -12;

    protected override bool IsCollapsable => false;
    protected override bool UseUniformButtonWidth => false;
    protected override string? Title => "Tools";

    public ToolsPanel(IEnumerable<IToolPaletteItem> items)
    {
        _buttons = items.Select(item => item.Button).ToArray();
    }

    protected override IEnumerable<ButtonBase> GetButtons()
        => _buttons;
}
