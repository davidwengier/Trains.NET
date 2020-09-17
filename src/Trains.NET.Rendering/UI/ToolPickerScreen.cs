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

        private readonly PaintBrush _border = new()
        {
            Color = Colors.Black,
            Style = PaintStyle.Stroke,
            StrokeWidth = 3
        };
        private readonly PaintBrush _panelBackground = new()
        {
            Color = Colors.White.WithAlpha("aa"),
            Style = PaintStyle.Fill
        };
        private readonly PaintBrush _background = new PaintBrush
        {
            Style = PaintStyle.Fill,
            Color = Colors.LightGray
        };
        private readonly PaintBrush _activeBackground = new PaintBrush
        {
            Style = PaintStyle.Fill,
            Color = Colors.LightBlue
        };
        private readonly PaintBrush _hoverBackground = new PaintBrush
        {
            Style = PaintStyle.Fill,
            Color = Colors.LightBlue.WithAlpha("55")
        };
        private readonly PaintBrush _label = new PaintBrush
        {
            TextSize = 13,
            IsAntialias = true,
            Color = Colors.Black
        };

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

            canvas.DrawRoundRect(-50, yPos, 50 + ButtonGap + ButtonWidth + 20, validTools.Count * ButtonGap * 2 + 20, 10, 10, _border);
            canvas.DrawRoundRect(-50, yPos, 50 + ButtonGap + ButtonWidth + 20, validTools.Count * ButtonGap * 2 + 20, 10, 10, _panelBackground);

            yPos += 20;

            foreach (ITool tool in validTools)
            {
                PaintBrush brush = tool == _hoverTool ? _hoverBackground
                            : tool == _gameManager.CurrentTool ? _activeBackground
                            : _background;
                canvas.DrawRect(ButtonGap, yPos, ButtonWidth, ButtonGap, brush);

                float textWidth = canvas.MeasureText(tool.Name, _label);

                canvas.DrawText(tool.Name, ButtonGap + ((ButtonWidth - textWidth) / 2), yPos + TextPadding, _label);
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
