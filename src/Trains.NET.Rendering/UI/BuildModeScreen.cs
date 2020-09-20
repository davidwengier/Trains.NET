using System;
using Trains.NET.Engine;

namespace Trains.NET.Rendering.UI
{
    [Order(110)]
    public class BuildModeScreen : IScreen
    {
        private const int TextPadding = 15;
        private const int ButtonHeight = 20;
        private const int ButtonWidth = 50;

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
            PaintBrush brush = _isHovered ? Brushes.ButtonHoverBackground : Brushes.ButtonBackground;
            canvas.DrawRect(0, 0, ButtonWidth, ButtonHeight, brush);

            string buttonText = _gameManager.BuildMode ? "Building" : "Playing";
            float textWidth = canvas.MeasureText(buttonText, Brushes.Label);

            canvas.DrawText(buttonText, ((ButtonWidth - textWidth) / 2), TextPadding, Brushes.Label);
        }
    }
}
