namespace Trains.NET.Engine;

[Order(20)]
public class EraserTool(
    ILayout trackLayout,
    IMovableLayout movableLayout,
    ITrainManager trainManager) : ITool
{
    private readonly ILayout _collection = trackLayout;
    private readonly IMovableLayout _movableLayout = movableLayout;
    private readonly ITrainManager _trainManager = trainManager;

    public ToolMode Mode => ToolMode.Build;
    public string Name => "Eraser";

    public void Execute(int column, int row, ExecuteInfo info)
    {
        _collection.Remove(column, row);
        if (_movableLayout.GetAt(column, row) is { } moveable)
        {
            _movableLayout.Remove(moveable);
            if (_trainManager.CurrentTrain == moveable)
            {
                _trainManager.CurrentTrain = null;
            }
        }
    }

    public bool IsValid(int column, int row) => true;
}
