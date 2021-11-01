using System.Collections.Generic;
using System.Data;
using System.Linq;
using Trains.NET.Engine;

namespace Trains.NET.Rendering.UI;

[Order(100)]
public class ToolsPanel : ButtonPanelBase
{
    private readonly IGameManager _gameManager;
    private readonly ButtonBase _switchModeButton;
    private readonly List<ButtonBase> _buildModeButtons;
    private readonly List<ButtonBase> _playModeButtons;

    protected override int Top => 60;

    protected override bool IsCollapsable => true;
    protected override string? Title => "Tools";

    public ToolsPanel(IEnumerable<ITool> tools, IGameManager gameManager)
    {
        _gameManager = gameManager;

        _gameManager.Changed += (s, e) => OnChanged();

        _switchModeButton = new BuildModeButton(_gameManager);

        _buildModeButtons = tools.Where(t => ShouldShowTool(true, t)).Select(tool => new TextButton(tool.Name, () => tool == _gameManager.CurrentTool, () => _gameManager.CurrentTool = tool)).ToList<ButtonBase>();
        _playModeButtons = tools.Where(t => ShouldShowTool(false, t)).Select(tool => new TextButton(tool.Name, () => tool == _gameManager.CurrentTool, () => _gameManager.CurrentTool = tool)).ToList<ButtonBase>();

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
