using System.Collections.Generic;
using System.Linq;
using Trains.NET.Engine;

namespace Trains.NET.Rendering.UI
{
    public class InteractionManager : IInteractionManager
    {
        private readonly IEnumerable<IScreen> _screens;
        private readonly IGame _game;
        private readonly IPixelMapper _pixelMapper;
        private readonly IGameManager _gameManager;
        private IScreen? _capturedScreen;

        public InteractionManager(IEnumerable<IScreen> screens, IGame game, IPixelMapper pixelMapper, IGameManager gameManager)
        {
            _screens = screens.Reverse().ToArray();
            _game = game;
            _pixelMapper = pixelMapper;
            _gameManager = gameManager;
        }

        public bool PointerClick(int x, int y)
        {
            if (HandleInteraction(x, y, MouseAction.Click))
            {
                return true;
            }

            return false;
        }

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
            (int width, int height) = _game.GetScreenSize();

            if (_capturedScreen != null)
            {
                _capturedScreen.HandleInteraction(x, y, width, height, action);
                return true;
            }

            foreach (IScreen screen in _screens)
            {
                if (screen.HandleInteraction(x, y, width, height, action))
                {
                    if (action != MouseAction.Move)
                    {
                        _capturedScreen = screen;
                    }
                    return true;
                }
            }


            if (_gameManager.CurrentTool is not null)
            {
                (int column, int row) = _pixelMapper.ViewPortPixelsToCoords(x, y);

                if (_gameManager.CurrentTool.IsValid(column, row) && action != MouseAction.Move)
                {
                    _gameManager.CurrentTool.Execute(column, row);
                    return true;
                }
                else if (_gameManager.CurrentTool is IDraggableTool draggableTool)
                {
                    if (action == MouseAction.Click)
                    {
                        draggableTool.StartDrag(x, y);
                        return true;
                    }
                    else if (action == MouseAction.Drag)
                    {
                        draggableTool.ContinueDrag(x, y);
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
