using System.Diagnostics.CodeAnalysis;

namespace Trains.NET.Engine.Tracks;

public class BridgeFactory(ITerrainMap terrainMap, ILayout<Track> trackLayout) : IStaticEntityFactory<Track>
{
    private readonly ITerrainMap _terrainMap = terrainMap;
    private readonly ILayout<Track> _trackLayout = trackLayout;

    public IEnumerable<Track> GetPossibleReplacements(int column, int row, Track track)
    {
        if (!_terrainMap.Get(column, row).IsWater)
        {
            yield break;
        }

        yield return new Bridge() { Direction = SingleTrackDirection.Horizontal };
        var neighbours = track.GetAllNeighbors();
        if (neighbours.Up is not null || neighbours.Down is not null)
        {
            yield return new Bridge() { Direction = SingleTrackDirection.Vertical };
        }
        if (neighbours.Up is not null && neighbours.Left is not null)
        {
            yield return new Bridge() { Direction = SingleTrackDirection.LeftUp };
        }
        if (neighbours.Up is not null && neighbours.Right is not null)
        {
            yield return new Bridge() { Direction = SingleTrackDirection.RightUp };
        }
        if (neighbours.Down is not null && neighbours.Left is not null)
        {
            yield return new Bridge() { Direction = SingleTrackDirection.LeftDown };
        }
        if (neighbours.Down is not null && neighbours.Right is not null)
        {
            yield return new Bridge() { Direction = SingleTrackDirection.RightDown };
        }
    }

    public bool TryCreateEntity(int column, int row, int fromColumn, int fromRow, [NotNullWhen(returnValue: true)] out Track? entity)
    {
        entity = null;

        if (!_terrainMap.Get(column, row).IsWater)
        {
            return false;
        }

        // this factory is never used to override
        if (_trackLayout.TryGet(column, row, out _))
        {
            return false;
        }

        entity = new Bridge();
        return true;
    }
}
