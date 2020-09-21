using System;
using Trains.NET.Engine;

namespace Trains.NET.Rendering.UI
{
    [Order(110)]
    public class BuildModeScreen : IScreen
    {
        private const int TextPadding = 15;
        private const int ButtonHeight = 20;
        private const int ButtonWidth = 60;

        private readonly IGameManager _gameManager;

        public event EventHandler? Changed;

        private bool _isHovered;

        public BuildModeScreen(IGameManager gameManager)
        {
            _gameManager = gameManager;
            _gameManager.Changed += (s, e) => Changed?.Invoke(s, e);
        }

        public bool HandleInteraction(int x, int y, int width, int height, MouseAction action)
        {
            if (action is MouseAction.Click or MouseAction.Move)
            {
                x -= 10;
                y -= 50;

                if (x is >= 0 and <= ButtonWidth)
                {
                    if (y >= 0 && y <= ButtonHeight)
                    {
                        if (action == MouseAction.Click)
                        {
                            _gameManager.BuildMode = !_gameManager.BuildMode;
                        }
                        else
                        {
                            _isHovered = true;
                        }
                        Changed?.Invoke(this, EventArgs.Empty);
                        return true;
                    }
                }
                _isHovered = false;
            }
            return false;
        }

        public void Render(ICanvas canvas, int width, int height)
        {
            canvas.Translate(10, 50);

            PaintBrush brush = _isHovered ? Brushes.ButtonHoverBackground : Brushes.ButtonBackground;
            canvas.DrawRoundRect(-5, 0, ButtonWidth + 10, ButtonHeight, 10, 10, Brushes.PanelBorder);
            canvas.DrawRoundRect(-5, 0, ButtonWidth + 10, ButtonHeight, 10, 10, brush);

            string buttonText = _gameManager.BuildMode ? "Building" : "Playing";
            float textWidth = canvas.MeasureText(buttonText, Brushes.Label);

            canvas.DrawText(buttonText, ((ButtonWidth - textWidth) / 2), TextPadding, Brushes.Label);
        }
    }
}
