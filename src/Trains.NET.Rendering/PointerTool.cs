using Trains.NET.Engine;

namespace Trains.NET.Rendering;

[Order(2)]
public class PointerTool(
    ITrainManager trainManager,
    IMovableLayout movableLayout,
    IPixelMapper pixelMapper,
    ILayout<Track> trackLayout) : IDraggableTool, IAlternateDragTool
{
    private readonly ITrainManager _trainManager = trainManager;
    private readonly IMovableLayout _movableLayout = movableLayout;
    private readonly IPixelMapper _pixelMapper = pixelMapper;
    private readonly ILayout<Track> _trackLayout = trackLayout;

    private int _lastX;
    private int _lastY;

    public ToolMode Mode => ToolMode.All;

    public string Name => "Pointer";

    public void Execute(int column, int row, ExecuteInfo info)
    {
        if (info.FromColumn != 0)
        {
            return;
        }

        if (_movableLayout.GetAt(column, row) is Train train)
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
        => _movableLayout.GetAt(column, row) is Train ||
        (_trackLayout.TryGet(column, row, out var track) && track.HasMultipleStates);
}
