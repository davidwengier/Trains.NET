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
        var depot = Depot.CreateNew(7);

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
