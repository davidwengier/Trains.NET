using System.Collections.Generic;
using System.Threading.Tasks;
using Trains.NET.Engine.Utilities;
using Trains.NET.Instrumentation;
using Trains.NET.Instrumentation.Stats;

namespace Trains.NET.Engine;

[Order(999999)]
public class GameStateManager : IGameStateManager, IInitializeAsync
{
    private readonly IEnumerable<IGameState> _gameStates;
    private readonly IGameStorage _storage;
    private readonly ITimer _timer;
    private readonly InformationStat _saveModeStat = InstrumentationBag.Add<InformationStat>("Save-Mode");
    private readonly ElapsedMillisecondsTimedStat _saveTimeStat = InstrumentationBag.Add<ElapsedMillisecondsTimedStat>("Save-Time");

    public SaveModes SaveMode { get; private set; }

    public GameStateManager(IEnumerable<IGameState> gameStates, IGameStorage storage, ITimer timer)
    {
        _gameStates = gameStates;
        _storage = storage;
        _timer = timer;
        _saveModeStat.Information = $"{this.SaveMode}";
    }

    public Task InitializeAsync(int columns, int rows)
    {
        Load();

        _timer.Interval = 1000;
        _timer.Elapsed += _timer_Elapsed;

        return Task.CompletedTask;
    }

    private void _timer_Elapsed(object? sender, System.EventArgs e)
    {
        Save();
    }

    public void Load()
    {
        foreach (var gameState in _gameStates)
        {
            if (!gameState.Load(_storage))
            {
                // If any one failed, reset the whole game because who knows what might happen
                Reset();
                break;
            }
        }
    }

    public void Reset()
    {
        foreach (var gameState in _gameStates)
        {
            gameState.Reset();
        }
    }

    public void Save()
    {
        using (_saveTimeStat.Measure())
        {
            foreach (var gameState in _gameStates)
            {
                gameState.Save(_storage);
            }
        }
    }

    public void ChangeSaveMode()
    {
        this.SaveMode = this.SaveMode switch
        {
            SaveModes.Manual => SaveModes.GameStep,
            SaveModes.GameStep => SaveModes.Timer,
            SaveModes.Timer => SaveModes.Manual,
            _ => SaveModes.Manual
        };
        _saveModeStat.Information = $"{this.SaveMode}";
        if (this.SaveMode == SaveModes.Timer)
            _timer.Start();
        else
            _timer.Stop();
    }
}
