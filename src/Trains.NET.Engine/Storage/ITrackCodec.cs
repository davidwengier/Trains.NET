using System.Collections.Generic;

namespace Trains.NET.Engine
{
    public interface ITrackCodec
    {
        string Encode(IEnumerable<Track> tracks);

        IEnumerable<Track> Decode(string input);
    }
}
