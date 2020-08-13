using System.Collections.Generic;

namespace Trains.NET.Engine
{
    public interface ITerrainSerializer
    { 
        IEnumerable<Terrain> Deserialize(string[] lines);
        string Serialize(IEnumerable<Terrain> terrain);
    }
}
