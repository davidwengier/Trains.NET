namespace Trains.NET.Engine;

public struct BasicPRNG(int seed)
{
    public int Seed = seed;
    private const int Multiplier = 16807; // Random-ish
    private const int Max = 2147483647; // Tested-ish
    private const float MaxFloat = Max;

    public int Next() => Seed = Math.Abs(Seed * Multiplier % Max);
    public float NextFloat() => Next() / MaxFloat;
    public float NextFloat(float min, float max) => NextFloat() * (max - min) + min;
    public int Next(int min, int max) => (int)NextFloat(min, max);
}
