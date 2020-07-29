using System.Collections.Generic;

namespace Trains.NET.Engine.Terrain
{
    public interface ITerrainMap : IEnumerable<TerrainCell>
    {
        void SetTerrainHeight(int column, int row, int height);

        TerrainCell GetAdjacentTerrain(TerrainCell terrain, TerrainAdjacency adjacency);

        void SetTerrainType(int column, int row, TerrainType water);
    }
}
