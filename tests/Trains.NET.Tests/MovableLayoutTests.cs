using Trains.NET.Engine;
using Trains.NET.Engine.Storage;
using Trains.NET.Engine.Trains;
using Trains.NET.Rendering;

namespace Trains.NET.Tests;

public class MovableLayoutTests
{
    [Theory]
    [InlineData(SignalState.Stop)]
    [InlineData(SignalState.TemporaryStop)]
    public async Task Update_WhenTrainStartsOnBlockedSignal_ClaimsSignal(SignalState signalState)
    {
        var serializer = new NullSerializer();
        var layout = new Layout(serializer);
        await layout.InitializeAsync(1, 1);
        var movableLayout = new MovableLayout(layout, serializer);
        var trackLayout = new FilteredLayout<Track>(layout);
        var trainManager = new TrainManager(movableLayout, layout);
        var trainTool = new TrainTool(movableLayout, trackLayout, trainManager);
        var signal = new Signal { SignalState = signalState };
        layout.Add(0, 0, signal);

        Assert.True(trainTool.IsValid(0, 0));
        trainTool.Execute(0, 0, new ExecuteInfo());
        var train = Assert.IsType<Train>(trainManager.CurrentTrain);

        movableLayout.Update(16);

        var lease = Assert.Single(movableLayout.LastTrackLeases);
        Assert.Same(signal, lease.Item1);
        Assert.Same(train, lease.Item2);
    }

    [Fact]
    public async Task Load_WhenTrainsShareTrack_SkipsDuplicateTrain()
    {
        var serializer = new EntityCollectionSerializer([new TrainSerializer()]);
        var layout = new Layout(serializer);
        await layout.InitializeAsync(1, 1);
        var movableLayout = new MovableLayout(layout, serializer);
        var trains = new Train[]
        {
            new(1) { Column = 0, Row = 0 },
            new(2) { Column = 0, Row = 0 }
        };
        var storage = new TestStorage(serializer.Serialize(trains));

        var loaded = movableLayout.Load(storage);

        Assert.True(loaded);
        var loadedTrain = Assert.IsType<Train>(Assert.Single(movableLayout));
        Assert.Equal(1, loadedTrain.Seed);
    }

    private sealed class TestStorage(string value) : IGameStorage
    {
        public string? Read(string key) => value;

        public void Write(string key, string value)
        {
        }
    }
}
