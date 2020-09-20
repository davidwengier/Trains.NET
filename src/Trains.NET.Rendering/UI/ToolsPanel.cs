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
        private readonly List<Button> _buttons;

        protected override int Top => 60;
        protected override int TopPadding => 20;

        public ToolsPanel(IEnumerable<ITool> tools, IGameManager gameManager)
        {
            _gameManager = gameManager;

            _gameManager.Changed += (s, e) => OnChanged();
            _buttons = tools.Select(tool => new Button(tool.Name, tool, () => tool == _gameManager.CurrentTool, () => _gameManager.CurrentTool = tool)).ToList();
        }

        protected override IEnumerable<Button> GetButtons()
            => _buttons.Where(t => ShouldShowTool(_gameManager.BuildMode, (ITool)t.Item)).ToList();

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
