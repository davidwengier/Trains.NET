using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trains.NET.Engine;
using Trains.NET.Engine.Storage;
using Trains.NET.Engine.Tracks;
using Trains.NET.Rendering;
using Xunit;
using Xunit.Abstractions;

namespace Trains.NET.Tests;

public class TestBase : IAsyncLifetime, IDisposable
{
    private int _lastCol;
    private int _lastRow;

    private readonly ITestOutputHelper _output;
    internal readonly TestTimer Timer;
    internal readonly GameBoard GameBoard;
    internal readonly Layout TrackLayout;
    internal readonly MovableLayout MovableLayout;
    internal readonly ITerrainMap TerrainMap;
    internal readonly ILayout<Track> FilteredLayout;
    internal readonly TrackTool TrackTool;
    internal readonly TrainManager TrainManager;

    protected TestBase(ITestOutputHelper output)
    {
        Timer = new TestTimer();
        var gameSerializer = new EntityCollectionSerializer(Enumerable.Empty<IEntitySerializer>());
        TrackLayout = new Layout(gameSerializer);
        TerrainMap = new FlatTerrainMap();
        MovableLayout = new MovableLayout(TrackLayout, gameSerializer);
        GameBoard = new GameBoard(new IGameStep[] {
            TrackLayout,
            MovableLayout
        }, new NullGameStateManager(), Timer);

        TrainManager = new TrainManager(MovableLayout, TrackLayout);

        FilteredLayout = new FilteredLayout<Track>(TrackLayout);

        var entityFactories = new List<IStaticEntityFactory<Track>>
            {
                new BridgeFactory(TerrainMap, FilteredLayout),
                new CrossTrackFactory(TerrainMap, TrackLayout),
                new TIntersectionFactory(TerrainMap, TrackLayout),
                new SingleTrackFactory(TerrainMap, FilteredLayout),
                new SignalFactory(TerrainMap)
            };

        TrackTool = new TrackTool(FilteredLayout, entityFactories);

        _output = output;
    }

    public async Task InitializeAsync()
    {
        await TrackLayout.InitializeAsync(100, 100);
        await GameBoard.InitializeAsync(100, 100);
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    protected void StartDrawTrack(int startColumn, int startRow)
    {
        _lastCol = startColumn;
        _lastRow = startRow;
        TrackTool.Execute(startColumn, startRow, new ExecuteInfo(0, 0));
    }

    protected void DrawTrack(DrawDirection step)
    {
        var (nextCol, nextRow) = step switch
        {
            DrawDirection.Up => (_lastCol, _lastRow - 1),
            DrawDirection.Down => (_lastCol, _lastRow + 1),
            DrawDirection.Left => (_lastCol - 1, _lastRow),
            DrawDirection.Right => (_lastCol + 1, _lastRow),
            _ => throw new InvalidOperationException()
        };

        TrackTool.Execute(nextCol, nextRow, new ExecuteInfo(_lastCol, _lastRow));
        _lastCol = nextCol;
        _lastRow = nextRow;
    }

    protected void AssertTrainMovement(float startAngle, int startColumn, int startRow, int endColumn, int endRow)
    {
        var train = TrainManager.AddTrain(startColumn, startRow) as Train;

        train!.LookaheadDistance = 0.1f;
        train.SetAngle(startAngle);

        _output.WriteLine("Initial: " + train);

        for (int i = 0; i < 100; i++)
        {
            Timer.Tick();
            _output.WriteLine($"Tick {i}: {train}");
        }

        Assert.Equal((endColumn, endRow), (train.Column, train.Row));
    }

    public void Dispose()
    {
        Timer.Dispose();
        GameBoard.Dispose();
    }
}
