using System.Collections.Generic;

namespace Trains.NET.Engine;

public interface IGameState
{
    bool Load(IEnumerable<IEntity> entities, int columns, int rows);
    IEnumerable<IEntity> Save();
    void Reset(int columns, int rows);
}
