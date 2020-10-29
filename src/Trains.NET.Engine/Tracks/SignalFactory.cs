using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Trains.NET.Engine.Tracks
{
    [Order(1000)]
    public class SignalFactory : IStaticEntityFactory<Track>
    {
        private readonly ITerrainMap _terrainMap;

        public SignalFactory(ITerrainMap terrainMap)
        {
            _terrainMap = terrainMap;
        }

        public IEnumerable<Track> GetPossibleReplacements(int column, int row, Track track)
        {
            if (!_terrainMap.Get(column, row).IsWater &&
                track.Direction is TrackDirection.Horizontal or TrackDirection.Vertical)
            {
                yield return new Signal() { Direction = track.Direction, SignalState = SignalState.Go };
                yield return new Signal() { Direction = track.Direction, SignalState = SignalState.TemporaryStop };
                yield return new Signal() { Direction = track.Direction, SignalState = SignalState.Stop };
            }
        }

        public bool TryCreateEntity(int column, int row, [NotNullWhen(returnValue: true)] out Track? entity)
        {
            // never automatically draw a signal

            entity = null;
            return false;
        }


    }
}
