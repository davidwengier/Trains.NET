using Trains.NET.Engine;
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
}
