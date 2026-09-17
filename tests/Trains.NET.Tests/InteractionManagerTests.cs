using Trains.NET.Engine;
using Trains.NET.Rendering;
using Trains.NET.Rendering.UI;

namespace Trains.NET.Tests;

public class InteractionManagerTests
{
    [Fact]
    public async Task PauseGameWhileDragging()
    {
        var tool = new PauseWhileDraggingTool();
        var gameStep = new TestGameStep();
        using var gameManager = new GameManager([tool], [gameStep], new TestTimer());
        var gameManagerChangeCount = 0;
        gameManager.Changed += (s, e) => gameManagerChangeCount++;
        var pixelMapper = new PixelMapper();
        await pixelMapper.InitializeAsync(10, 10);
        var interactionManager = new InteractionManager(
            [],
            new TestGame(),
            pixelMapper,
            gameManager,
            new AlternateDragTool(),
            new TooltipScreen());

        interactionManager.PointerClick(50, 50);
        gameManager.GameLoopStep();

        Assert.False(gameManager.Paused);
        Assert.Equal(1, gameStep.UpdateCount);

        interactionManager.PointerDrag(90, 50);
        gameManager.GameLoopStep();

        Assert.True(gameManager.Paused);
        Assert.Equal(1, gameStep.UpdateCount);
        Assert.Equal(1, gameManagerChangeCount);

        interactionManager.PointerRelease(90, 50);
        gameManager.GameLoopStep();

        Assert.False(gameManager.Paused);
        Assert.Equal(2, gameStep.UpdateCount);
        Assert.Equal(2, gameManagerChangeCount);
    }

    private sealed class PauseWhileDraggingTool : ITool
    {
        public bool PauseGameDuringDrag => true;

        public string Name => "Test";

        public void Execute(int column, int row, ExecuteInfo info)
        {
        }

        public bool IsValid(int column, int row) => true;
    }

    private sealed class TestGameStep : IGameStep
    {
        public int UpdateCount { get; private set; }

        public void Update(long millisecondsSinceLastUpdate)
        {
            UpdateCount++;
        }
    }

    private sealed class AlternateDragTool : IAlternateDragTool
    {
        public void StartDrag(int x, int y)
        {
        }

        public void ContinueDrag(int x, int y)
        {
        }
    }

    private sealed class TestGame : IGame
    {
        public void AdjustViewPortIfNecessary()
        {
        }

        public void Dispose()
        {
        }

        public (int Width, int Height) GetScreenSize() => (100, 100);

        public (int Width, int Height) GetSize() => (100, 100);

        public Task InitializeAsync(int columns, int rows) => Task.CompletedTask;

        public void Render(ICanvas canvas)
        {
        }

        public void SetContext(IContext context)
        {
        }

        public void SetDisplayScale(float scaleX, float scaleY)
        {
        }

        public void SetSize(int width, int height)
        {
        }
    }
}
