using System;

namespace Trains.NET.Engine
{
    public class Terrain
    {
        public const int MaxHeight = 100;

        private const int NumberWaterLevels = 2;
        private const int NumberSandLevels = 1; // Not really important, but good to remember
        private const int NumberLandLevels = 3;
        private const int NumberMountainLevels = 2;

        private const int TotalLevels = NumberWaterLevels + NumberSandLevels + NumberLandLevels + NumberMountainLevels;
        private const int HeightPerLevel = MaxHeight / TotalLevels + 1;
        private const int LastWaterHeight = NumberWaterLevels * HeightPerLevel - 1;
        private const int FirstMountainHeight = (TotalLevels - NumberMountainLevels) * HeightPerLevel;
        private const int TotalLandLevels = TotalLevels - NumberWaterLevels;

        public int Column { get; set; }
        public int Row { get; set; }
        public int Height { get; set; }

        public bool IsWater => this.Height <= LastWaterHeight;

        public bool IsLand => !this.IsWater;

        public bool IsMountain => this.Height >= FirstMountainHeight;

        public float GetScaleFactor()
        {
            const float minimumScaling = 0.5f;
            const float maximumScaling = 1.0f;

            // We only want to deal with water level and up
            int height = Math.Max(this.Height, LastWaterHeight);

            float heightRange = MaxHeight - LastWaterHeight;
            float heightDelta = height - LastWaterHeight;

            // This will give us a value of 0 to 1 for scaling
            float heightScalingFactor = heightDelta / heightRange;

            // We want to lock this into a number of discrete bands to make caching easier
            float bandedScalingFactor = ((int)(TotalLandLevels * heightScalingFactor)) / (float)TotalLandLevels;

            float delta = maximumScaling - minimumScaling;

            // Apply the scaling factor to this and add it back to the lower bound
            float scaledValue = (delta * bandedScalingFactor) + minimumScaling;

            return scaledValue;
        }
    }
}
