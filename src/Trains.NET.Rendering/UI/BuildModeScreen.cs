using System;
using Trains.NET.Engine;

namespace Trains.NET.Rendering.UI
{
    [Order(110)]
    public class BuildModeScreen : IScreen, IInteractionHandler
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

        public bool HandlePointerAction(int x, int y, int width, int height, PointerAction action)
        {
            if (action is PointerAction.Click or PointerAction.Move)
            {
                x -= 10;
                y -= 50;

                if (x is >= 0 and <= ButtonWidth)
                {
                    if (y >= 0 && y <= ButtonHeight)
                    {
                        if (action == Rendering.PointerAction.Click)
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

            canvas.DrawRoundRect(-5, 0, ButtonWidth + 10, ButtonHeight, 10, 10, Brushes.ButtonBackground);
            if (_isHovered)
            {
                canvas.DrawRoundRect(-5, 0, ButtonWidth + 10, ButtonHeight, 10, 10, Brushes.ButtonHoverBackground);
            }
            canvas.DrawRoundRect(-5, 0, ButtonWidth + 10, ButtonHeight, 10, 10, Brushes.PanelBorder);

            string buttonText = _gameManager.BuildMode ? "Building" : "Playing";
            float textWidth = canvas.MeasureText(buttonText, Brushes.Label);

            canvas.DrawText(buttonText, ((ButtonWidth - textWidth) / 2), TextPadding, Brushes.Label);
        }
    }
}
