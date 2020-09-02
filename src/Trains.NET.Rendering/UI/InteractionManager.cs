using System.Collections.Generic;

namespace Trains.NET.Rendering.UI
{
    public class InteractionManager : IInteractionManager
    {
        private readonly IEnumerable<IScreen> _screens;
        private readonly IGame _game;
        private IScreen? _capturedScreen;

        public InteractionManager(IEnumerable<IScreen> screens, IGame game)
        {
            _screens = screens;
            _game = game;
        }

        public bool PointerClick(int x, int y)
            => HandleInteraction(x, y, MouseAction.Click);

        public bool PointerMove(int x, int y)
            => HandleInteraction(x, y, MouseAction.Move);

        public bool PointerDrag(int x, int y)
            => HandleInteraction(x, y, MouseAction.Drag);

        public bool PointerRelease(int x, int y)
        {
            if (_capturedScreen != null)
            {
                _capturedScreen = null;
                return true;
            }
            return false;
        }

        private bool HandleInteraction(int x, int y, MouseAction action)
        {
            var (width, height) = _game.GetScreenSize();

            if (_capturedScreen != null)
            {
                _capturedScreen.HandleInteraction(x, y, width, height, action);
                return true;
            }

            foreach (var screen in _screens)
            {
                if (screen.HandleInteraction(x, y, width, height, action))
                {
                    _capturedScreen = screen;
                    return true;
                }
            }

            return false;
        }
    }
}
