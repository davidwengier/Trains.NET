using Trains.NET.Engine;
using Trains.NET.Rendering;

namespace Trains.NET
{
    public static class TerrainExtensions
    {
        public static bool IsWater(this Terrain terrain)
            => terrain.Height <= TerrainColourLookup.GetWaterLevel();

        public static bool IsLand(this Terrain terrain)
            => !terrain.IsWater();
    }
}
