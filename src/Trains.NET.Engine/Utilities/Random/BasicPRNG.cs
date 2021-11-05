using System;

namespace Trains.NET.Engine;

public struct BasicPRNG
{
    public int Seed;
    private const int Multiplier = 16807; // Random-ish
    private const int Max = 2147483647; // Tested-ish
    private const float MaxFloat = Max;

    public BasicPRNG(int seed)
    {
        Seed = seed;
    }

    public int Next() => Seed = Math.Abs(Seed * Multiplier % Max);
    public float NextFloat() => Next() / MaxFloat;
    public float NextFloat(float min, float max) => NextFloat() * (max - min) + min;
    public int Next(int min, int max) => (int)NextFloat(min, max);
}
