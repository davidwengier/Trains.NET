using System.Collections.Generic;

namespace Trains.NET.Engine
{
    public interface IEntityCollectionSerializer
    {
        IEnumerable<IEntity> Deserialize(string[] lines);
        string Serialize(IEnumerable<IEntity> tracks);
    }
}
