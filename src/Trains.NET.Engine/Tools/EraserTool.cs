namespace Trains.NET.Engine;

[Order(20)]
public class EraserTool : ITool
{
    private readonly ILayout _collection;
    private readonly IMovableLayout _movableLayout;
    private readonly ITrainManager _trainManager;

    public ToolMode Mode => ToolMode.Build;
    public string Name => "Eraser";

    public EraserTool(ILayout trackLayout, IMovableLayout movableLayout, ITrainManager trainManager)
    {
        _collection = trackLayout;
        _movableLayout = movableLayout;
        _trainManager = trainManager;
    }

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
