using Trains.NET.Engine;

namespace Trains.NET.Rendering.UI
{
    internal class BuildModeButton : Button
    {
        private const int Size = 34;
        private static readonly PaintBrush s_label = Brushes.Label with
        {
            TextSize = 20
        };

        private readonly IGameManager _gameManager;
        private bool _buildHovered;
        private bool _playHovered;

        public BuildModeButton(Engine.IGameManager gameManager)
        {
            _gameManager = gameManager;
            this.Height = Size;
            this.Width = Size * 2;
        }

        public override int GetMinimumWidth(ICanvas canvas)
        {
            return Size * 2;
        }

        public override bool HandleMouseAction(int x, int y, PointerAction action)
        {
            if (x is >= 0 && x <= Size && y >= 0 && y <= this.Height)
            {
                if (action == PointerAction.Click)
                {
                    _gameManager.BuildMode = true;
                }
                else
                {
                    _buildHovered = true;
                }
                _playHovered = false;
                return true;
            }
            else if (x is >= Size + 1 && x <= Size * 2 && y >= 0 && y <= this.Height)
            {
                if (action == PointerAction.Click)
                {
                    _gameManager.BuildMode = false;
                }
                else
                {
                    _playHovered = true;
                }
                _buildHovered = false;
                return true;
            }
            _buildHovered = false;
            _playHovered = false;
            return false;
        }

        public override void Render(ICanvas canvas)
        {
            DrawButton(canvas, Size, Size, "{{fa-wrench}}", _gameManager.BuildMode, _buildHovered, s_label);
            canvas.Translate(Size + 1, 0);
            DrawButton(canvas, Size, Size, "{{fa-play}}", !_gameManager.BuildMode, _playHovered, s_label);
        }
    }
}
