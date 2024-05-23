using Trains.NET.Engine;

namespace Trains.NET.Rendering.UI;

public class InteractionManager(
    IEnumerable<IInteractionHandler> handlers,
    IGame game,
    IPixelMapper pixelMapper,
    IGameManager gameManager,
    IAlternateDragTool alternateDragTool) : IInteractionManager
{
    private readonly IEnumerable<IInteractionHandler> _handler = handlers.Reverse().ToArray();
    private readonly IGame _game = game;
    private readonly IPixelMapper _pixelMapper = pixelMapper;
    private readonly IGameManager _gameManager = gameManager;
    private readonly IAlternateDragTool _alternateDragTool = alternateDragTool;
    private IInteractionHandler? _capturedHandler;
    private ITool? _capturedTool;
    private bool _hasDragged;
    private int _lastToolColumn;
    private int _lastToolRow;

    public bool PointerClick(int x, int y)
        => HandleInteraction(x, y, PointerAction.Click);

    public bool PointerMove(int x, int y)
        => HandleInteraction(x, y, PointerAction.Move);

    public bool PointerDrag(int x, int y)
        => HandleInteraction(x, y, PointerAction.Drag);

    public bool PointerAlternateClick(int x, int y)
        => HandleInteraction(x, y, PointerAction.AlternateClick);

    public bool PointerAlternateDrag(int x, int y)
        => HandleInteraction(x, y, PointerAction.AlternateDrag);

    public bool PointerZoomIn(int x, int y)
        => HandleInteraction(x, y, PointerAction.ZoomIn);

    public bool PointerZoomOut(int x, int y)
        => HandleInteraction(x, y, PointerAction.ZoomOut);

    public bool PointerRelease(int x, int y)
    {
        (int column, int row) = _pixelMapper.ViewPortPixelsToCoords(x, y);

        if (_capturedHandler is null &&
            !_hasDragged &&
            _gameManager.CurrentTool is not null &&
            _gameManager.CurrentTool.IsValid(column, row))
        {
            _gameManager.CurrentTool.Execute(column, row, new ExecuteInfo());
        }

        _hasDragged = false;
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

        if (action is PointerAction.Click)
        {
            bool preHandled = false;
            foreach (IInteractionHandler handler in _handler)
            {
                if (handler.PreHandleNextClick && action is PointerAction.Click)
                {
                    _capturedHandler = handler;
                    preHandled |= handler.HandlePointerAction(x, y, width, height, action);
                }
            }
            if (preHandled)
            {
                return true;
            }
        }
        else if (action is PointerAction.AlternateClick)
        {
            _alternateDragTool.StartDrag(x, y);
            return true;
        }
        else if (action is PointerAction.AlternateDrag)
        {
            _hasDragged = true;
            _alternateDragTool.ContinueDrag(x, y);
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

        if (action is PointerAction.Click)
        {
            _hasDragged = false;
        }
        if (action is PointerAction.Drag)
        {
            _hasDragged = true;
        }

        try
        {
            if (!inSameCell && action is PointerAction.Drag && tool.IsValid(column, row))
            {
                tool.Execute(column, row, new ExecuteInfo(
                    fromColumn: _lastToolColumn,
                    fromRow: _lastToolRow));
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
        }
        finally
        {
            if (_hasDragged)
            {
                _lastToolColumn = column;
                _lastToolRow = row;
            }
        }
        return false;
    }
}
