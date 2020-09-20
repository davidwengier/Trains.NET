using System;
using System.Collections.Generic;
using System.Linq;
using Trains.NET.Engine;

namespace Trains.NET.Rendering.UI
{
    [Order(100)]
    public class ToolPickerScreen : IScreen
    {
        private const int TextPadding = 15;
        private const int ButtonGap = 20;
        private const int ButtonWidth = 50;

        private readonly IEnumerable<ITool> _tools;
        private readonly IGameManager _gameManager;

        public event EventHandler? Changed;

        private ITool? _hoverTool;

        public ToolPickerScreen(IEnumerable<ITool> tools, IGameManager gameManager)
        {
            _tools = tools;
            _gameManager = gameManager;
            _gameManager.Changed += (s, e) => Changed?.Invoke(s, e);
        }

        public bool HandleInteraction(int x, int y, int width, int height, MouseAction action)
        {
            if (action is MouseAction.Click or MouseAction.Move)
            {
                var validTools = _tools.Where(t => ShouldShowTool(_gameManager.BuildMode, t)).ToList();
                int yPos = ButtonGap * 3;

                if (x is >= 0 and <= ButtonGap + ButtonWidth + 20 && y >= yPos && y <= yPos + validTools.Count * ButtonGap * 2 + 20)
                {
                    yPos += 20;
                    foreach (ITool tool in _tools.Where(t => ShouldShowTool(_gameManager.BuildMode, t)))
                    {
                        if (x is >= ButtonGap and <= ButtonGap + ButtonWidth && y >= yPos && y <= yPos + ButtonGap)
                        {
                            if (action == MouseAction.Click)
                            {
                                _gameManager.CurrentTool = tool;
                            }
                            else
                            {
                                _hoverTool = tool;
                            }
                            Changed?.Invoke(this, EventArgs.Empty);
                            return true;
                        }

                        yPos += ButtonGap + ButtonGap;
                    }
                    return true;
                }
                _hoverTool = null;
            }
            return false;
        }

        public void Render(ICanvas canvas, int width, int height)
        {
            int yPos = ButtonGap * 3;

            var validTools = _tools.Where(t => ShouldShowTool(_gameManager.BuildMode, t)).ToList();

            canvas.DrawRoundRect(-50, yPos, 50 + ButtonGap + ButtonWidth + 20, validTools.Count * ButtonGap * 2 + 20, 10, 10, Brushes.PanelBorder);
            canvas.DrawRoundRect(-50, yPos, 50 + ButtonGap + ButtonWidth + 20, validTools.Count * ButtonGap * 2 + 20, 10, 10, Brushes.PanelBackground);

            yPos += 20;

            foreach (ITool tool in validTools)
            {
                PaintBrush brush = tool == _hoverTool ? Brushes.ButtonHoverBackground
                            : tool == _gameManager.CurrentTool ? Brushes.ButtonActiveBackground
                            : Brushes.ButtonBackground;
                canvas.DrawRect(ButtonGap, yPos, ButtonWidth, ButtonGap, brush);

                float textWidth = canvas.MeasureText(tool.Name, Brushes.Label);

                canvas.DrawText(tool.Name, ButtonGap + ((ButtonWidth - textWidth) / 2), yPos + TextPadding, Brushes.Label);
                yPos += ButtonGap + ButtonGap;
            }
        }

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
