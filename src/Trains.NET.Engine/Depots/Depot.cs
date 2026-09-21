namespace Trains.NET.Engine;

public class Depot : IStaticEntity
{
    private Depot(int trainSeed, DepotDirection direction, bool hasProducedTrain)
    {
        TrainSeed = trainSeed;
        Direction = direction;
        HasProducedTrain = hasProducedTrain;
    }

    public static Depot CreateNew(int trainSeed)
        => new(trainSeed, DepotDirection.Right, false);

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

    public void Created()
    {
    }

    public void Removed()
    {
    }

    public void Updated()
    {
    }

    public void Replaced()
    {
    }

    public void Stored(ILayout? collection)
    {
    }
}
