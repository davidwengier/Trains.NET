using System;
using Trains.NET.Engine.Terrain;

namespace Trains.NET.Rendering.Terrain
{
    public static class TerrainColourMapper
    {
        public static Color GetColourForTerrainType(TerrainType terrainType)
        {
            return terrainType switch
            {
                TerrainType.Grass => Colors.LightGreen,
                TerrainType.Sand => Colors.LightYellow,
                TerrainType.Water => Colors.LightBlue,
                TerrainType.Empty => Colors.Empty,
                _ => throw new NotImplementedException()
            };
        }
    }
}
