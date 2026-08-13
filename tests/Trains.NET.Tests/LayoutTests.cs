using Trains.NET.Engine;

namespace Trains.NET.Tests;

public class LayoutTests
{
    [Theory]
    [InlineData(3, 0)]
    [InlineData(0, 2)]
    public async Task CoordinatesAtDimensionsAreOutOfBounds(int column, int row)
    {
        var layout = new Layout(new NullSerializer());
        await layout.InitializeAsync(3, 2);
        var track = new SingleTrack();

        layout.Add(column, row, track);
        layout.Set(column, row, track);
        layout.Remove(column, row);

        Assert.False(layout.TryGet(column, row, out _));
    }

    [Fact]
    public async Task CoordinatesAbovePreviousLimitAreValid()
    {
        var layout = new Layout(new NullSerializer());
        await layout.InitializeAsync(202, 202);
        var track = new SingleTrack();

        layout.Add(201, 201, track);

        Assert.True(layout.TryGet(201, 201, out var actual));
        Assert.Same(track, actual);
    }
}
