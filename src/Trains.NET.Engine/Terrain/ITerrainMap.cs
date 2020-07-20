using System.Collections.Generic;

namespace Trains.NET.Engine.Tracks
{
    public interface ITerrainMap : IEnumerable<Terrain>
    {
        void SetTerrainHeight(int column, int row, int height);

        Terrain GetAdjacentTerrainUp(Terrain terrain);
        Terrain GetAdjacentTerrainDown(Terrain terrain);
        Terrain GetAdjacentTerrainLeft(Terrain terrain);
        Terrain GetAdjacentTerrainRight(Terrain terrain);
        Terrain GetAdjacentTerrainUpLeft(Terrain terrain);
        Terrain GetAdjacentTerrainUpRight(Terrain terrain);
        Terrain GetAdjacentTerrainDownLeft(Terrain terrain);
        Terrain GetAdjacentTerrainDownRight(Terrain terrain);

        void SetTerrainType(int column, int row, TerrainType water);
    }
}
