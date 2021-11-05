using System.Collections.Generic;

namespace Trains.NET.Engine;

public interface IGameState
{
    bool Load(IEnumerable<IEntity> entities);
    IEnumerable<IEntity> Save();
    void Reset();
}
