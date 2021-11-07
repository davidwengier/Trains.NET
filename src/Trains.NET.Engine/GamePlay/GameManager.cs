using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trains.NET.Instrumentation;

namespace Trains.NET.Engine;

public class GameManager : IGameManager, IInitializeAsync
{
    private const int GameLoopInterval = 16;

    private bool _buildMode;
    private ITool _currentTool;
    private readonly ITool _defaultTool;
    private readonly ITimer _gameLoopTimer;
    private readonly IGameStateManager _gameStateManager;
    private readonly IEnumerable<IGameStep> _gameSteps;
    private readonly ElapsedMillisecondsTimedStat _gameUpdateTime = InstrumentationBag.Add<ElapsedMillisecondsTimedStat>("Game-LoopStepTime");

    public event EventHandler? Changed;

    public bool BuildMode
    {
        get { return _buildMode; }
        set
        {
            _buildMode = value;
            _currentTool = _defaultTool;

            Changed?.Invoke(this, EventArgs.Empty);
        }
    }

    public ITool CurrentTool
    {
        get { return _currentTool; }
        set
        {
            _currentTool = value;

            Changed?.Invoke(this, EventArgs.Empty);
        }
    }

    public GameManager(IEnumerable<ITool> tools, IEnumerable<IGameStep> gameSteps, ITimer timer, IGameStateManager gameStateManager)
    {
        _defaultTool = tools.First();
        _currentTool = _defaultTool;

        _gameLoopTimer = timer;
        _gameSteps = gameSteps;

        _gameLoopTimer.Interval = GameLoopInterval;
        _gameLoopTimer.Elapsed += GameLoopTimerElapsed;
        _gameStateManager = gameStateManager;
    }

    public Task InitializeAsync(int columns, int rows)
    {
        _gameLoopTimer.Start();

        return Task.CompletedTask;
    }

    public void GameLoopStep()
    {
        if (_buildMode) return;

        using (_gameUpdateTime.Measure())
        {
            var timeSinceLastTick = _gameLoopTimer?.TimeSinceLastTick ?? 16;
            foreach (var gameStep in _gameSteps)
            {
                gameStep.Update(timeSinceLastTick);
                _gameStateManager.Save();
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
