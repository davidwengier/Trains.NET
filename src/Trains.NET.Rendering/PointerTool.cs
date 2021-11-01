using Trains.NET.Engine;

namespace Trains.NET.Rendering;

[Order(2)]
public class PointerTool : IDraggableTool, IAlternateDragTool
{
    private readonly ITrainManager _trainManager;
    private readonly IGameBoard _gameBoard;
    private readonly IPixelMapper _pixelMapper;
    private readonly ILayout<Track> _trackLayout;

    private int _lastX;
    private int _lastY;

    public ToolMode Mode => ToolMode.All;
    public PointerTool(ITrainManager trainManager, IGameBoard gameBoard, IPixelMapper pixelMapper, ILayout<Track> trackLayout)
    {
        _trainManager = trainManager;
        _gameBoard = gameBoard;
        _pixelMapper = pixelMapper;
        _trackLayout = trackLayout;
    }

    public string Name => "Pointer";

    public void Execute(int column, int row, ExecuteInfo info)
    {
        if (info.FromColumn != 0)
        {
            return;
        }

        if (_gameBoard.GetMovableAt(column, row) is Train train)
        {
            _trainManager.CurrentTrain = train;
        }
        else
        {
            if (_trackLayout.TryGet(column, row, out Track? track))
            {
                track.NextState();
            }
        }
    }

    public void StartDrag(int x, int y)
    {
        _lastX = x;
        _lastY = y;
    }

    public void ContinueDrag(int x, int y)
    {
        _pixelMapper.AdjustViewPort(x - _lastX, y - _lastY);
        _lastX = x;
        _lastY = y;
    }

    public bool IsValid(int column, int row)
        => _gameBoard.GetMovableAt(column, row) is Train ||
        (_trackLayout.TryGet(column, row, out var track) && track.HasMultipleStates);
}
