using System.Collections.Generic;
using System.Linq;

namespace Trains.NET.Engine;

public class GameStateManager : IGameStateManager
{
    private readonly IEnumerable<IGameState> _gameStates;
    private readonly IGameStorage _storage;
    private readonly IGameSerializer _gameSerializer;

    public GameStateManager(IEnumerable<IGameState> gameStates, IGameStorage storage, IGameSerializer gameSerializer)
    {
        _gameStates = gameStates;
        _storage = storage;
        _gameSerializer = gameSerializer;
    }

    public void Load(int columns, int rows)
    {
        var entitiesString = _storage.ReadEntities();
        bool failed = false;
        if (entitiesString is not null)
        {
            var entities = _gameSerializer.Deserialize(entitiesString);

            foreach (var gameState in _gameStates)
            {
                if (!gameState.Load(entities, columns, rows))
                {
                    failed = true;
                    break;
                }
            }
        }
        else
        {
            failed = true;
        }

        if (failed)
        {
            Reset(columns, rows);
        }
    }

    public void Reset(int columns, int rows)
    {
        foreach (var gameState in _gameStates)
        {
            gameState.Reset(columns, rows);
        }
    }

    public void Save()
    {
        var entities = _gameStates.SelectMany(x => x.Save());

        var serializedEntities = _gameSerializer.Serialize(entities);

        _storage.WriteEntities(serializedEntities);
    }
}
