using System.Collections.Generic;
using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    public static class TerrainColourLookup
    {  
        public static Color DefaultColour => Colors.Green;
        private static Color WaterToLandTransition = Colors.LightYellow;

        private static readonly List<Color> _heightOrderedColours = new List<Color> {
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
            int heightPerTerrainType = (Terrain.MaxHeight / _heightOrderedColours.Count) + 1;
            return _heightOrderedColours.IndexOf(WaterToLandTransition) * heightPerTerrainType - 1;
        }

        public static Color GetTerrainColour(Terrain terrain)
        {
            int heightPerTerrainType = (Terrain.MaxHeight / _heightOrderedColours.Count) + 1;

            int colourLookup = terrain.Height / heightPerTerrainType;
            return _heightOrderedColours[colourLookup];
        } 
        
        public static int GetLandColourCount()
        {
            var firstLand = _heightOrderedColours.IndexOf(WaterToLandTransition);
            return _heightOrderedColours.Count - firstLand;
        }
    }
}
