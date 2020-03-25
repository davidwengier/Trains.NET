using System.Collections.Generic;

namespace Trains.NET.Engine
{
    public interface ITrackSerializer
    {
        IEnumerable<Track> Deserialize(string[] lines);
        string Serialize(IEnumerable<Track> tracks);
    }
}
