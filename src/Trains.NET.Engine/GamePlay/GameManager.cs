using Trains.NET.Instrumentation;

namespace Trains.NET.Engine;

// Start the game loop after GameStateManager has finished loading persisted state.
[Order(1000000)]
public class GameManager : IGameManager, IInitializeAsync
{
    private const int GameLoopInterval = 16;

    private ITool _currentTool;
    private readonly ITimer _gameLoopTimer;
    private readonly IEnumerable<IGameStep> _gameSteps;
    private readonly ElapsedMillisecondsTimedStat _gameUpdateTime = InstrumentationBag.Add<ElapsedMillisecondsTimedStat>("Game-LoopStepTime");

    public event EventHandler? Changed;

    public bool IsPaused { get; set; }

    public ITool CurrentTool
    {
        get { return _currentTool; }
        set
        {
            _currentTool = value;

            Changed?.Invoke(this, EventArgs.Empty);
        }
    }

    public GameManager(IEnumerable<ITool> tools, IEnumerable<IGameStep> gameSteps, ITimer timer)
    {
        _currentTool = tools.First();

        _gameLoopTimer = timer;
        _gameSteps = gameSteps;

        _gameLoopTimer.Interval = GameLoopInterval;
        _gameLoopTimer.Elapsed += GameLoopTimerElapsed;
    }

    public Task InitializeAsync(int columns, int rows)
    {
        _gameLoopTimer.Start();

        return Task.CompletedTask;
    }

    public void GameLoopStep()
    {
        if (IsPaused) return;

        using (_gameUpdateTime.Measure())
        {
            var timeSinceLastTick = _gameLoopTimer?.TimeSinceLastTick ?? 16;
            foreach (var gameStep in _gameSteps)
            {
                gameStep.Update(timeSinceLastTick);
            }
        }
    }

    private void GameLoopTimerElapsed(object? sender, EventArgs e)
        => GameLoopStep();

    public void Dispose()
    {
        _gameLoopTimer.Dispose();
    }
}
