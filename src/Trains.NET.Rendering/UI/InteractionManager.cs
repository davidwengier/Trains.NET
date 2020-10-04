using System.Collections.Generic;
using System.Linq;
using Trains.NET.Engine;

namespace Trains.NET.Rendering.UI
{
    public class InteractionManager : IInteractionManager
    {
        private readonly IEnumerable<IInteractionHandler> _handler;
        private readonly IGame _game;
        private readonly IPixelMapper _pixelMapper;
        private readonly IGameManager _gameManager;
        private IInteractionHandler? _capturedScreen;

        public InteractionManager(IEnumerable<IInteractionHandler> handlers, IGame game, IPixelMapper pixelMapper, IGameManager gameManager)
        {
            _handler = handlers.Reverse().ToArray();
            _game = game;
            _pixelMapper = pixelMapper;
            _gameManager = gameManager;
        }

        public bool PointerClick(int x, int y)
        {
            if (HandleInteraction(x, y, PointerAction.Click))
            {
                return true;
            }

            return false;
        }

        public bool PointerMove(int x, int y)
            => HandleInteraction(x, y, PointerAction.Move);

        public bool PointerDrag(int x, int y)
            => HandleInteraction(x, y, PointerAction.Drag);

        public bool PointerRelease(int x, int y)
        {
            if (_capturedScreen != null)
            {
                _capturedScreen = null;
                return true;
            }
            return false;
        }

        public bool PointerZoomIn(int x, int y)
            => HandleInteraction(x, y, PointerAction.ZoomIn);

        public bool PointerZoomOut(int x, int y)
            => HandleInteraction(x, y, PointerAction.ZoomOut);

        private bool HandleInteraction(int x, int y, PointerAction action)
        {
            (int width, int height) = _game.GetScreenSize();

            if (_capturedScreen != null)
            {
                _capturedScreen.HandlePointerAction(x, y, width, height, action);
                return true;
            }

            foreach (IInteractionHandler handler in _handler)
            {
                if (handler.HandlePointerAction(x, y, width, height, action))
                {
                    if (action is PointerAction.Click or PointerAction.Drag)
                    {
                        _capturedScreen = handler;
                    }
                    return true;
                }
            }

            if (_gameManager.CurrentTool is not null)
            {
                (int column, int row) = _pixelMapper.ViewPortPixelsToCoords(x, y);

                if (_gameManager.CurrentTool.IsValid(column, row) && action != PointerAction.Move)
                {
                    _gameManager.CurrentTool.Execute(column, row);
                    return true;
                }
                else if (_gameManager.CurrentTool is IDraggableTool draggableTool)
                {
                    if (action == PointerAction.Click)
                    {
                        draggableTool.StartDrag(x, y);
                        return true;
                    }
                    else if (action == PointerAction.Drag)
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
