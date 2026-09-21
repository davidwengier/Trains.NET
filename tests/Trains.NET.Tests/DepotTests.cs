using Trains.NET.Engine;
using Trains.NET.Engine.Storage;

namespace Trains.NET.Tests;

public class DepotTests
{
    [Fact]
    public async Task FactoryCreatesRightFacingDepotWithoutTrack()
    {
        var layout = await CreateLayout();
        var factory = new DepotFactory(layout, new FlatTerrainMap());

        var created = factory.TryCreateEntity(1, 2, 0, 0, out var depot);

        Assert.True(created);
        Assert.NotNull(depot);
        Assert.Equal(DepotDirection.Right, depot.Direction);
    }

    [Fact]
    public async Task FactoryPrefersFirstConnectedTrackClockwiseFromRight()
    {
        var layout = await CreateLayout();
        var factory = new DepotFactory(layout, new FlatTerrainMap());
        layout.Set(2, 1, new SingleTrack { Direction = SingleTrackDirection.Horizontal });
        layout.Set(1, 2, new SingleTrack { Direction = SingleTrackDirection.Vertical });
        layout.Set(0, 1, new SingleTrack { Direction = SingleTrackDirection.Horizontal });
        layout.Set(1, 0, new SingleTrack { Direction = SingleTrackDirection.Vertical });

        var created = factory.TryCreateEntity(1, 1, 0, 0, out var depot);

        Assert.True(created);
        Assert.NotNull(depot);
        Assert.Equal(DepotDirection.Right, depot.Direction);
    }

    [Fact]
    public async Task FactorySkipsTrackThatDoesNotConnectToDepotCell()
    {
        var layout = await CreateLayout();
        var factory = new DepotFactory(layout, new FlatTerrainMap());
        layout.Set(2, 1, new SingleTrack { Direction = SingleTrackDirection.RightUp });
        layout.Set(1, 2, new SingleTrack { Direction = SingleTrackDirection.Vertical });

        var created = factory.TryCreateEntity(1, 1, 0, 0, out var depot);

        Assert.True(created);
        Assert.NotNull(depot);
        Assert.Equal(DepotDirection.Down, depot.Direction);
    }

    [Fact]
    public async Task SpawnerProducesConfiguredTrainOnce()
    {
        var layout = await CreateLayout();
        var depotLayout = new FilteredLayout<Depot>(layout);
        var trackLayout = new FilteredLayout<Track>(layout);
        var movableLayout = new MovableLayout(layout, new EntityCollectionSerializer([]));
        var trainManager = new TrainManager(movableLayout, layout);
        var spawner = new DepotTrainSpawner(depotLayout, trackLayout, movableLayout, trainManager);
        var depot = Depot.Rehydrate(7, DepotDirection.Right, false);

        layout.Add(1, 2, depot);
        layout.Add(2, 2, new SingleTrack { Direction = SingleTrackDirection.Horizontal });

        spawner.Update(16);
        spawner.Update(16);

        var train = Assert.IsType<Train>(Assert.Single(movableLayout));
        Assert.Equal(7, train.Seed);
        Assert.Equal(1, train.Carriages);
        Assert.Equal(0, train.Angle);
        Assert.True(depot.HasProducedTrain);
    }

    [Fact]
    public async Task SpawnerWaitsForTrackConnectedToDepotOpening()
    {
        var layout = await CreateLayout();
        var depotLayout = new FilteredLayout<Depot>(layout);
        var trackLayout = new FilteredLayout<Track>(layout);
        var movableLayout = new MovableLayout(layout, new EntityCollectionSerializer([]));
        var trainManager = new TrainManager(movableLayout, layout);
        var spawner = new DepotTrainSpawner(depotLayout, trackLayout, movableLayout, trainManager);
        var depot = Depot.CreateNew(7, DepotDirection.Right);

        layout.Add(1, 2, depot);
        layout.Set(2, 2, new SingleTrack { Direction = SingleTrackDirection.RightUp });

        spawner.Update(16);

        Assert.Empty(movableLayout);
        Assert.False(depot.HasProducedTrain);

        layout.Set(2, 2, new SingleTrack { Direction = SingleTrackDirection.LeftUp });

        spawner.Update(16);

        Assert.Single(movableLayout);
        Assert.True(depot.HasProducedTrain);
    }

    [Theory]
    [InlineData(DepotDirection.Right, 0)]
    [InlineData(DepotDirection.Up, 270)]
    [InlineData(DepotDirection.Left, 180)]
    [InlineData(DepotDirection.Down, 90)]
    public void RotationMatchesDirection(DepotDirection direction, float expectedRotation)
    {
        var depot = Depot.Rehydrate(1, direction, false);

        Assert.Equal(expectedRotation, depot.Rotation);
    }

    [Fact]
    public async Task TrackShapesTowardExistingDepotOpening()
    {
        var layout = await CreateLayout();
        layout.Add(2, 3, Depot.Rehydrate(1, DepotDirection.Up, false));
        var track = new SingleTrack();

        layout.Add(2, 2, track);

        Assert.Equal(SingleTrackDirection.Vertical, track.Direction);
    }

    [Fact]
    public async Task PlacingDepotReshapesExistingTrack()
    {
        var layout = await CreateLayout();
        var track = new SingleTrack();
        layout.Add(2, 2, track);

        layout.Add(2, 3, Depot.Rehydrate(1, DepotDirection.Up, false));

        Assert.Equal(SingleTrackDirection.Vertical, track.Direction);
    }

    [Fact]
    public async Task RotatingDepotRefreshesExistingTrack()
    {
        var layout = await CreateLayout();
        var track = new SingleTrack();
        layout.Add(2, 2, track);
        var depot = Depot.Rehydrate(1, DepotDirection.Up, false);
        layout.Add(2, 3, depot);
        Assert.Equal(SingleTrackDirection.Vertical, track.Direction);

        layout.Set(2, 3, depot.WithDirection(DepotDirection.Right));

        Assert.Equal(SingleTrackDirection.Horizontal, track.Direction);
    }

    [Fact]
    public async Task DepotConnectionDoesNotBecomeMovementNeighbor()
    {
        var layout = await CreateLayout();
        layout.Add(1, 2, Depot.CreateNew(1, DepotDirection.Right));
        layout.Add(2, 2, new SingleTrack());

        var movementNeighbors = TrackNeighbors.GetConnectedNeighbours(layout, 2, 2);

        Assert.Null(movementNeighbors.Left);
    }

    [Fact]
    public void SerializerPreservesProductionState()
    {
        var serializer = new DepotSerializer();
        var depot = Depot.Rehydrate(42, DepotDirection.Up, true);

        Assert.True(serializer.TrySerialize(depot, out var data));
        Assert.True(serializer.TryDeserialize(data, out var deserialized));

        var restoredDepot = Assert.IsType<Depot>(deserialized);
        Assert.Equal(42, restoredDepot.TrainSeed);
        Assert.Equal(DepotDirection.Up, restoredDepot.Direction);
        Assert.True(restoredDepot.HasProducedTrain);
    }

    private static async Task<Layout> CreateLayout()
    {
        var layout = new Layout(new EntityCollectionSerializer([]));
        await layout.InitializeAsync(5, 5);
        return layout;
    }
}
