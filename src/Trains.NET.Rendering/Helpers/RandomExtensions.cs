using System;

namespace Trains.NET.Rendering
{
    internal static class RandomExtensions
    {
        public static float NextFloat(this Random r, float min, float max) => (float)(r.NextDouble() * (max - min) + min);
    }
}
