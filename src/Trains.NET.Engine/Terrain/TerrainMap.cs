using System.Collections;
using System.Collections.Generic;

namespace Trains.NET.Engine.Tracks
{
    internal class TerrainMap : ITerrainMap
    {
        private readonly Dictionary<(int, int), Terrain> _terrainMap = new();

        public void AddTerrain(Terrain terrain)
        {
            (int Column, int Row) key = (terrain.Column, terrain.Row);
            _terrainMap[key] = terrain;
        }

        public IEnumerator<Terrain> GetEnumerator()
        {
            return _terrainMap.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Terrain GetAdjacentTerrainUp(Terrain terrain)
        {
            return GetTerrainOrDefault(terrain.Column, terrain.Row - 1);
        }

         public Terrain GetAdjacentTerrainDown(Terrain terrain)
        {
            return GetTerrainOrDefault(terrain.Column, terrain.Row + 1);
        }

        public Terrain GetAdjacentTerrainLeft(Terrain terrain)
        {
            return GetTerrainOrDefault(terrain.Column - 1, terrain.Row);
        }

        public Terrain GetAdjacentTerrainRight(Terrain terrain)
        {
            return GetTerrainOrDefault(terrain.Column + 1, terrain.Row);
        }

        private Terrain GetTerrainOrDefault(int column, int row)
        {
            if (_terrainMap.TryGetValue((column, row), out Terrain? adjacentTerrain))
            {
                return adjacentTerrain;
            }

            return new Terrain
            {
                Row = row,
                Column = column,
                Altitude = 0
            };
        }
    }
}
