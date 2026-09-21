namespace Trains.NET.Engine;

[Order(-100)]
public class DepotTrainSpawner(
    ILayout<Depot> depotLayout,
    ILayout<Track> trackLayout,
    IMovableLayout movableLayout,
    ITrainManager trainManager) : IGameStep
{
    private readonly ILayout<Depot> _depotLayout = depotLayout;
    private readonly ILayout<Track> _trackLayout = trackLayout;
    private readonly IMovableLayout _movableLayout = movableLayout;
    private readonly ITrainManager _trainManager = trainManager;

    public void Update(long timeSinceLastTick)
    {
        foreach (var depot in _depotLayout.Where(depot => !depot.HasProducedTrain))
        {
            (var column, var row) = depot.GetTrackPosition();
            if (!_trackLayout.TryGet(column, row, out var track) ||
                !depot.CanEmitTrainOnto(track) ||
                _movableLayout.GetAt(column, row) is not null)
            {
                continue;
            }

            if (_trainManager.AddTrain(column, row, depot.TrainSeed) is not Train train)
            {
                continue;
            }

            train.SetAngle(depot.Rotation);
            depot.HasProducedTrain = true;
            _depotLayout.ToLayout().RaiseCollectionChanged();
        }
    }
}
