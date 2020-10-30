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
        private IInteractionHandler? _capturedHandler;
        private ITool? _capturedTool;
        private int _lastToolColumn;
        private int _lastToolRow;

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
            _lastToolColumn = -1;
            _lastToolRow = -1;
            if (_capturedHandler != null || _capturedTool != null)
            {
                _capturedTool = null;
                _capturedHandler = null;
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

            if (_capturedHandler != null)
            {
                _capturedHandler.HandlePointerAction(x, y, width, height, action);
                return true;
            }
            else if (_capturedTool != null)
            {
                ExecuteTool(_capturedTool, x, y, action);
                return true;
            }

            foreach (IInteractionHandler handler in _handler)
            {
                if (handler.HandlePointerAction(x, y, width, height, action))
                {
                    if (action is PointerAction.Click or PointerAction.Drag)
                    {
                        _capturedHandler = handler;
                    }
                    return true;
                }
            }

            if (_gameManager.CurrentTool is not null)
            {
                if (ExecuteTool(_gameManager.CurrentTool, x, y, action))
                {
                    if (action is PointerAction.Click or PointerAction.Drag)
                    {
                        _capturedTool = _gameManager.CurrentTool;
                    }
                }
            }

            return false;
        }

        private bool ExecuteTool(ITool tool, int x, int y, PointerAction action)
        {
            (int column, int row) = _pixelMapper.ViewPortPixelsToCoords(x, y);

            var inSameCell = (column == _lastToolColumn && row == _lastToolRow);

            if (action is PointerAction.Click or PointerAction.Drag)
            {
                _lastToolColumn = column;
                _lastToolRow = row;
            }

            if (!inSameCell && action is PointerAction.Click or PointerAction.Drag && tool.IsValid(column, row))
            {
                tool.Execute(column, row, action is PointerAction.Drag);
                return true;
            }
            else if (tool is IDraggableTool draggableTool)
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

            return false;
        }
    }
}
