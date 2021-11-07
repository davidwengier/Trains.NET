using System.Collections.Generic;
using System.Threading.Tasks;

namespace Trains.NET.Engine;

[Order(999999)]
public class GameStateManager : IGameStateManager, IInitializeAsync
{
    private readonly IEnumerable<IGameState> _gameStates;
    private readonly IGameStorage _storage;
    private readonly ITimer _timer;

    public GameStateManager(IEnumerable<IGameState> gameStates, IGameStorage storage, ITimer timer)
    {
        _gameStates = gameStates;
        _storage = storage;
        _timer = timer;
    }

    public Task InitializeAsync(int columns, int rows)
    {
        Load();

        _timer.Interval = 1000;
        _timer.Elapsed += _timer_Elapsed;
        //Uncomment this line to enable saving once per second/interval time set above.
        //On my clunky VM, enabling this increased GameLoopStep time from ~45ms to ~50ms, but that might just be noise and I didn't measure precisely
        //_timer.Start();

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
        foreach (var gameState in _gameStates)
        {
            gameState.Save(_storage);
        }
    }
}
