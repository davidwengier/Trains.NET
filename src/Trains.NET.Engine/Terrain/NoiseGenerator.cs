using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Trains.NET.Engine
{
    public static class NoiseGenerator
    {
        // Copied from https://stackoverflow.com/questions/8659351/2d-perlin-noise
        public static Dictionary<(int x, int y), float> GenerateNoiseMap(int width, int height, int octaves)
        {
            float[] data = new float[width * height];

            /// track min and max noise value. Used to normalize the result to the 0 to 1.0 range.
            var min = float.MaxValue;
            var max = float.MinValue;

            /// rebuild the permutation table to get a different noise pattern. 
            /// Leave this out if you want to play with changing the number of octaves while 
            /// maintaining the same overall pattern.
            Noise2d.Reseed();

            var frequency = 0.5f;
            var amplitude = 1f;

            for (var octave = 0; octave < octaves; octave++)
            {
                /// parallel loop - easy and fast.
                Parallel.For(0
                    , width * height
                    , (offset) =>
                    {
                        var i = offset % width;
                        var j = offset / width;
                        var noise = Noise2d.Noise(i * frequency * 1f / width, j * frequency * 1f / height);
                        noise = data[j * width + i] += noise * amplitude;

                        min = Math.Min(min, noise);
                        max = Math.Max(max, noise);

                    }
                );

                frequency *= 2;
                amplitude /= 2;
            }

            var points = data.Select((f) => (f - min) / (max - min)).ToList();

            return points.Select((f,i) => (Value: f, Index: i)).ToDictionary(k => (x: k.Index % width, y: k.Index / width), k => k.Value);
        }
    }
}
