using System.Collections.Generic;

namespace Trains.NET.Engine
{
    public interface ITerrainMap : IEnumerable<Terrain>
    {
        void Set(IEnumerable<Terrain> terrainList);

        void SetTerrainHeight(int column, int row, int height);

        Terrain GetAdjacentTerrainUp(Terrain terrain);
        Terrain GetAdjacentTerrainDown(Terrain terrain);
        Terrain GetAdjacentTerrainLeft(Terrain terrain);
        Terrain GetAdjacentTerrainRight(Terrain terrain);
        void SetTerrainType(int column, int row, TerrainType water);
    }
}
