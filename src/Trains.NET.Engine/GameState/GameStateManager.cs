using System.Collections.Generic;
using System.Threading.Tasks;

namespace Trains.NET.Engine;

[Order(999999)]
public class GameStateManager : IGameStateManager, IInitializeAsync
{
    private readonly IEnumerable<IGameState> _gameStates;
    private readonly IGameStorage _storage;

    public GameStateManager(IEnumerable<IGameState> gameStates, IGameStorage storage)
    {
        _gameStates = gameStates;
        _storage = storage;
    }

    public Task InitializeAsync(int columns, int rows)
    {
        Load();

        return Task.CompletedTask;
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
