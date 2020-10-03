using System.Collections.Generic;
using System.Data;
using System.Linq;
using Trains.NET.Engine;

namespace Trains.NET.Rendering.UI
{
    [Order(100)]
    public class ToolsPanel : ButtonPanelBase
    {
        private readonly IGameManager _gameManager;
        private readonly List<Button> _buildModeButtons;
        private readonly List<Button> _playModeButtons;

        protected override int Top => 60;
        protected override int TopPadding => 20;

        protected override string? Title => "Tools";

        public ToolsPanel(IEnumerable<ITool> tools, IGameManager gameManager)
        {
            _gameManager = gameManager;

            _gameManager.Changed += (s, e) => OnChanged();
            _buildModeButtons = tools.Where(t => ShouldShowTool(true, t)).Select(tool => new Button(tool.Name, () => tool == _gameManager.CurrentTool, () => _gameManager.CurrentTool = tool)).ToList();
            _playModeButtons = tools.Where(t => ShouldShowTool(false, t)).Select(tool => new Button(tool.Name, () => tool == _gameManager.CurrentTool, () => _gameManager.CurrentTool = tool)).ToList();
        }

        protected override IEnumerable<Button> GetButtons()
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
}
