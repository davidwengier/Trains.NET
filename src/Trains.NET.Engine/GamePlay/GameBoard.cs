using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trains.NET.Instrumentation;

namespace Trains.NET.Engine;

// Ensure this gets initialized last
[Order(999999)]
public class GameBoard : IGameBoard, IInitializeAsync
{
    private readonly ElapsedMillisecondsTimedStat _gameUpdateTime = InstrumentationBag.Add<ElapsedMillisecondsTimedStat>("Game-LoopStepTime");

    private const int GameLoopInterval = 16;

    private readonly ITimer _gameLoopTimer;
    private readonly IGameStateManager _gameStateManager;
    private readonly IEnumerable<IGameStep> _gameSteps;

    public bool Enabled { get; set; } = true;

    public GameBoard(IEnumerable<IGameStep> gameSteps, IGameStateManager gameStateManager, ITimer timer)
    {
        _gameLoopTimer = timer;
        _gameSteps = gameSteps;
        _gameStateManager = gameStateManager;

        _gameLoopTimer.Interval = GameLoopInterval;
        _gameLoopTimer.Elapsed += GameLoopTimerElapsed;
    }

    public Task InitializeAsync(int columns, int rows)
    {
        _gameStateManager.Load();
        _gameLoopTimer.Start();

        return Task.CompletedTask;
    }

    public void GameLoopStep()
    {
        if (!this.Enabled) return;

        using (_gameUpdateTime.Measure())
        {
            var timeSinceLastTick = _gameLoopTimer?.TimeSinceLastTick ?? 16;
            foreach (var gameStep in _gameSteps)
            {
                gameStep.Update(timeSinceLastTick);
            }
        }
    }

    private void GameLoopTimerElapsed(object? sender, EventArgs e) => GameLoopStep();

    public void ClearAll()
        => _gameStateManager.Reset();

    public void Dispose()
    {
        _gameLoopTimer.Dispose();
        _gameStateManager.Save();
    }
}
