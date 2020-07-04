using System.Collections.Generic;

namespace Trains.NET.Engine.Tracks
{
    public interface ITerrainMap : IEnumerable<Terrain>
    {
        void AddTerrain(Terrain terrain);

        Terrain GetAdjacentTerrainUp(Terrain terrain);
        Terrain GetAdjacentTerrainDown(Terrain terrain);
        Terrain GetAdjacentTerrainLeft(Terrain terrain);
        Terrain GetAdjacentTerrainRight(Terrain terrain);
    }
}
