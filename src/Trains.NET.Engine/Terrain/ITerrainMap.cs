using System;
using System.Collections.Generic;

namespace Trains.NET.Engine
{
    public interface ITerrainMap : IEnumerable<Terrain>
    {
        event EventHandler CollectionChanged;
        void Set(IEnumerable<Terrain> terrainList);

        void SetTerrainHeight(int column, int row, int height);

        Terrain GetAdjacentTerrainUp(Terrain terrain);
        Terrain GetAdjacentTerrainDown(Terrain terrain);
        Terrain GetAdjacentTerrainLeft(Terrain terrain);
        Terrain GetAdjacentTerrainRight(Terrain terrain);
        Terrain GetTerrainOrDefault(int column, int row);
        void Reset(int seed, int columns, int rows);
        bool IsEmpty();
    }
}
