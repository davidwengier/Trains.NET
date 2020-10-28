using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Trains.NET.Engine.Tracks
{
    [Order(1000)]
    public class SignalFactory : IStaticEntityFactory<Track>
    {
        private readonly ITerrainMap _terrainMap;
        private readonly ILayout<Track> _layout;

        public SignalFactory(ITerrainMap terrainMap, ILayout<Track> layout)
        {
            _terrainMap = terrainMap;
            _layout = layout;
        }

        public IEnumerable<Track> GetAllPossibleEntities(int column, int row)
        {
            if (!_terrainMap.Get(column, row).IsWater &&
                _layout.TryGet(column, row, out Track? track) &&
                track.Direction is TrackDirection.Horizontal or TrackDirection.Vertical)
            {

                yield return new Signal() { Direction = track.Direction, SignalState = SignalState.Go };
                yield return new Signal() { Direction = track.Direction, SignalState = SignalState.TemporaryStop };
                yield return new Signal() { Direction = track.Direction, SignalState = SignalState.Stop };
            }
        }

        public bool TryCreateEntity(int column, int row, [NotNullWhen(returnValue: true)] out Track? entity)
        {
            if (!_terrainMap.Get(column, row).IsWater &&
                _layout.TryGet(column, row, out Track? track) &&
                (
                    track is Signal ||
                    track.Direction is TrackDirection.Horizontal or TrackDirection.Vertical && track.Happy)
                )
            {
                entity = new Signal();
                return true;
            }

            entity = null;
            return false;
        }


    }
}
