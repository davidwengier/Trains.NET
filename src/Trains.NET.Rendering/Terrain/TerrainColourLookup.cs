using System.Collections.Generic;
using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    public static class TerrainColourLookup
    {  
        public static Color DefaultColour => Colors.Green;
        private static readonly Color s_waterToLandTransition = Colors.LightYellow;
        private static readonly Color s_landToMountainTransition = Colors.Gray;

        private static readonly List<Color> s_heightOrderedColours = new List<Color> {
                Colors.DarkBlue,
                Colors.LightBlue,
                Colors.LightYellow,
                Colors.LightGreen,
                Colors.Green,
                Colors.DarkGreen,
                Colors.Gray,
                Colors.DirtyWhite
        };

        public static int GetWaterLevel()
        {
            int heightPerTerrainType = (Terrain.MaxHeight / s_heightOrderedColours.Count) + 1;
            return s_heightOrderedColours.IndexOf(s_waterToLandTransition) * heightPerTerrainType - 1;
        }

        public static int GetMountainLevel()
        {
            int heightPerTerrainType = (Terrain.MaxHeight / s_heightOrderedColours.Count) + 1;
            return s_heightOrderedColours.IndexOf(s_landToMountainTransition) * heightPerTerrainType;
        }

        public static Color GetTerrainColour(Terrain terrain)
        {
            int heightPerTerrainType = (Terrain.MaxHeight / s_heightOrderedColours.Count) + 1;

            int colourLookup = terrain.Height / heightPerTerrainType;
            return s_heightOrderedColours[colourLookup];
        } 
        
        public static int GetLandColourCount()
        {
            int firstLand = s_heightOrderedColours.IndexOf(s_waterToLandTransition);
            return s_heightOrderedColours.Count - firstLand;
        }
    }
}
