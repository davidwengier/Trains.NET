using Trains.NET.Engine;

namespace Trains.NET.Rendering;

[Order(50)]
public class TrainTool : ITool
{
    private readonly IMovableLayout _movableLayout;
    private readonly ILayout<Track> _trackLayout;
    private readonly ITrainManager _trainManager;

    public ToolMode Mode => ToolMode.Play;
    public string Name => "Train";

    public TrainTool(IMovableLayout movableLayout, ILayout<Track> trackLayout, ITrainManager gameState)
    {
        _movableLayout = movableLayout;
        _trackLayout = trackLayout;
        _trainManager = gameState;
    }

    public void Execute(int column, int row, ExecuteInfo info)
    {
        if (_trainManager.AddTrain(column, row) is Train train)
        {
            _trainManager.CurrentTrain = train;
        }
    }

    public bool IsValid(int column, int row) => _trackLayout.TryGet(column, row, out _) &&
        _movableLayout.GetAt(column, row) == null;
}
