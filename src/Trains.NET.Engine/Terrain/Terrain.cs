using System;

namespace Trains.NET.Engine;

public class Terrain
{
    public const int MaxHeight = 100;

    public const int NumberWaterLevels = 4;
    public const int NumberSandLevels = 1; // Not really important, but good to remember
    public const int NumberLandLevels = 5;
    public const int NumberMountainLevels = 3;

    public const int TotalLevels = NumberWaterLevels + NumberSandLevels + NumberLandLevels + NumberMountainLevels;
    public const int HeightPerLevel = MaxHeight / TotalLevels + 1;
    public const int LastWaterHeight = NumberWaterLevels * HeightPerLevel - 1;
    public const int FirstLandHeight = LastWaterHeight + 1;
    public const int FirstMountainHeight = (TotalLevels - NumberMountainLevels) * HeightPerLevel;
    public const int TotalLandLevels = TotalLevels - NumberWaterLevels;

    public int Column { get; set; }
    public int Row { get; set; }
    public int Height { get; set; }

    public int TerrainLevel => this.Height / HeightPerLevel;
    public bool IsWater => this.Height <= LastWaterHeight;
    public bool IsLand => !this.IsWater;
    public bool IsMountain => this.Height >= FirstMountainHeight;

    /// <summary>
    /// Gets a value between 0 and 1 that maps to the position in the possible range of terrain
    /// </summary>
    /// <returns></returns>
    public float GetScaleFactor()
    {
        const float minimumScaling = 0.5f;
        const float maximumScaling = 1.0f;

        // We only want to deal with water level and up
        int height = Math.Max(this.Height, FirstLandHeight);

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
