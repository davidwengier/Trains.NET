using System.Collections.Generic;

namespace Trains.NET.Engine
{
    public interface IGameStorage
    {
        IEnumerable<Track> ReadTracks();

        void WriteTracks(IEnumerable<Track> tracks);
    }
}
