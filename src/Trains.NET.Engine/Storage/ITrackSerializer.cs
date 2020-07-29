using System.Collections.Generic;

namespace Trains.NET.Engine
{
    public interface ITrackSerializer
    {
        IEnumerable<IStaticEntity> Deserialize(string[] lines);
        string Serialize(IEnumerable<IStaticEntity> tracks);
    }
}
