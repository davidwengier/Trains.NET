using Trains.NET.Engine;

namespace Trains.NET.Rendering.UI;

[Order(100)]
public class ToolsPanel : ButtonPanelBase
{
    private readonly IGameManager _gameManager;
    private readonly ButtonBase _switchModeButton;
    private readonly List<ButtonBase> _buildModeButtons;
    private readonly List<ButtonBase> _playModeButtons;

    protected override int Top => -12;

    protected override bool IsCollapsable => false;
    protected override bool UseUniformButtonWidth => false;
    protected override string? Title => "Mode";

    public ToolsPanel(IEnumerable<IToolPaletteItem> items, IGameManager gameManager)
    {
        _gameManager = gameManager;

        _gameManager.Changed += (s, e) => OnChanged();

        _switchModeButton = new BuildModeButton(_gameManager);

        _buildModeButtons = items.Where(item => ShouldShowTool(true, item.Tool)).Select(item => item.Button).ToList();
        _playModeButtons = items.Where(item => ShouldShowTool(false, item.Tool)).Select(item => item.Button).ToList();

        _buildModeButtons.Insert(0, _switchModeButton);
        _playModeButtons.Insert(0, _switchModeButton);
    }

    protected override IEnumerable<ButtonBase> GetButtons()
        => _gameManager.BuildMode ? _buildModeButtons : _playModeButtons;

    private static bool ShouldShowTool(bool buildMode, ITool tool)
        => (buildMode, tool.Mode) switch
        {
            (true, ToolMode.Build) => true,
            (false, ToolMode.Play) => true,
            (_, ToolMode.All) => true,
            _ => false
        };
}
