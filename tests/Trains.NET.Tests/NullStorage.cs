using System.Collections.Generic;
using System.Linq;
using Trains.NET.Engine;

namespace Trains.NET.Tests
{
    internal class NullStorage : IGameStorage
    {
        public IEnumerable<Track> ReadTracks()
        {
            return Enumerable.Empty<Track>();
        }

        public void WriteTracks(IEnumerable<Track> tracks)
        {
        }
    }
}
