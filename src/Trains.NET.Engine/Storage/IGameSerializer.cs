using System.Collections.Generic;

namespace Trains.NET.Engine;

public interface IGameSerializer
{
    IEnumerable<IEntity> Deserialize(string lines);
    string Serialize(IEnumerable<IEntity> tracks);
}
