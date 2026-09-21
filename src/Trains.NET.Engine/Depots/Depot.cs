namespace Trains.NET.Engine;

public class Depot : IStaticEntity, ITrackConnection
{
    private ILayout? _layout;

    private Depot(int trainSeed, DepotDirection direction, bool hasProducedTrain)
    {
        TrainSeed = trainSeed;
        Direction = direction;
        HasProducedTrain = hasProducedTrain;
    }

    public static Depot CreateNew(int trainSeed, DepotDirection direction)
        => new(trainSeed, direction, false);

    public static Depot Rehydrate(int trainSeed, DepotDirection direction, bool hasProducedTrain)
        => new(trainSeed, direction, hasProducedTrain);

    public Depot WithDirection(DepotDirection direction)
        => new(TrainSeed, direction, HasProducedTrain);

    public int Column { get; set; }
    public int Row { get; set; }
    public int TrainSeed { get; }
    public DepotDirection Direction { get; }
    public bool HasProducedTrain { get; set; }
    public float Rotation
        => Direction switch
        {
            DepotDirection.Right => 0,
            DepotDirection.Up => 270,
            DepotDirection.Left => 180,
            DepotDirection.Down => 90,
            _ => throw new InvalidOperationException($"Unknown depot direction: {Direction}")
        };

    public string Identifier => Direction.ToString();

    public (int Column, int Row) GetTrackPosition()
        => Direction switch
        {
            DepotDirection.Left => (Column - 1, Row),
            DepotDirection.Up => (Column, Row - 1),
            DepotDirection.Right => (Column + 1, Row),
            DepotDirection.Down => (Column, Row + 1),
            _ => throw new InvalidOperationException($"Unknown depot direction: {Direction}")
        };

    public bool CanEmitTrainOnto(Track track)
        => Direction switch
        {
            DepotDirection.Right => track.IsConnectedLeft(),
            DepotDirection.Up => track.IsConnectedDown(),
            DepotDirection.Left => track.IsConnectedRight(),
            DepotDirection.Down => track.IsConnectedUp(),
            _ => false
        };

    public bool IsConnectedRight() => Direction == DepotDirection.Right;
    public bool IsConnectedDown() => Direction == DepotDirection.Down;
    public bool IsConnectedLeft() => Direction == DepotDirection.Left;
    public bool IsConnectedUp() => Direction == DepotDirection.Up;

    public bool CanConnectRight() => IsConnectedRight();
    public bool CanConnectDown() => IsConnectedDown();
    public bool CanConnectLeft() => IsConnectedLeft();
    public bool CanConnectUp() => IsConnectedUp();

    public void Created()
    {
        RefreshNeighbors();
    }

    public void Removed()
    {
        RefreshNeighbors();
    }

    public void Updated()
    {
    }

    public void Replaced()
    {
        RefreshNeighbors();
    }

    public void Stored(ILayout? collection)
    {
        _layout = collection;
    }

    private void RefreshNeighbors()
    {
        if (_layout is null)
        {
            return;
        }

        UpdateNeighbor(Column - 1, Row);
        UpdateNeighbor(Column, Row - 1);
        UpdateNeighbor(Column + 1, Row);
        UpdateNeighbor(Column, Row + 1);
    }

    private void UpdateNeighbor(int column, int row)
    {
        if (_layout!.TryGet(column, row, out var entity))
        {
            entity.Updated();
        }
    }
}
