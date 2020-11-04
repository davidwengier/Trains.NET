using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Trains.NET.Engine.Tracks
{
    [Order(4)]
    public class SignalFactory : IStaticEntityFactory<Track>
    {
        private readonly ITerrainMap _terrainMap;

        public SignalFactory(ITerrainMap terrainMap)
        {
            _terrainMap = terrainMap;
        }

        public IEnumerable<Track> GetPossibleReplacements(int column, int row, Track track)
        {
            if (!_terrainMap.Get(column, row).IsWater)
            {
                var neighbours = track.GetAllNeighbors();
                if (neighbours.Left is not null || neighbours.Right is not null)
                {
                    yield return new Signal() { Direction = TrackDirection.Horizontal, SignalState = SignalState.Go };
                    yield return new Signal() { Direction = TrackDirection.Horizontal, SignalState = SignalState.TemporaryStop };
                    yield return new Signal() { Direction = TrackDirection.Horizontal, SignalState = SignalState.Stop };
                }
                if (neighbours.Up is not null || neighbours.Down is not null)
                {
                    yield return new Signal() { Direction = TrackDirection.Vertical, SignalState = SignalState.Go };
                    yield return new Signal() { Direction = TrackDirection.Vertical, SignalState = SignalState.TemporaryStop };
                    yield return new Signal() { Direction = TrackDirection.Vertical, SignalState = SignalState.Stop };
                }
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
