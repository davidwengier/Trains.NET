using System.Numerics;

namespace Trains.NET.Engine;

internal static class NoiseGenerator
{
    // Copied from https://stackoverflow.com/questions/8659351/2d-perlin-noise
    public static Dictionary<(int x, int y), float> GenerateNoiseMap(int width, int height, int octaves, int seed)
    {
        var random = new Random(seed);
        var randomData = GetRandomData(random);
        var gradients = CalculateGradients(random);

        var data = new float[width * height];

        var min = float.MaxValue;
        var max = float.MinValue;

        var frequency = 2f;
        var amplitude = 0.1f;

        for (var octave = 0; octave < octaves; octave++)
        {
            for (var offset = 0; offset < width * height; offset++)
            {
                var i = offset % width;
                var j = offset / width;
                var noise = Noise(i * frequency * 1f / width, j * frequency * 1f / height, randomData, gradients);
                noise = data[j * width + i] += noise * amplitude;

                min = Math.Min(min, noise);
                max = Math.Max(max, noise);
            }

            frequency *= 2;
            amplitude /= 2;
        }

        var points = data.Select((f) => (f - min) / (max - min)).ToList();

        return points.Select((f, i) => (Value: f, Index: i)).ToDictionary(k => (x: k.Index % width, y: k.Index / width), k => k.Value);

        static int[] GetRandomData(Random random)
        {
            var p = Enumerable.Range(0, 256).ToArray();

            for (var i = 0; i < p.Length; i++)
            {
                var source = random.Next(p.Length);

                var t = p[i];
                p[i] = p[source];
                p[source] = t;
            }

            return p;
        }

        static Vector2[] CalculateGradients(Random random)
        {
            var grad = new Vector2[256];

            for (var i = 0; i < grad.Length; i++)
            {
                Vector2 gradient;
                do
                {
                    gradient = new Vector2((float)(random.NextDouble() * 2 - 1), (float)(random.NextDouble() * 2 - 1));
                }
                while (gradient.LengthSquared() >= 1);

                gradient = Vector2.Normalize(gradient);

                grad[i] = gradient;
            }

            return grad;
        }
    }

    private static float Drop(float t)
    {
        t = Math.Abs(t);
        return 1f - t * t * t * (t * (t * 6 - 15) + 10);
    }

    private static float Q(float u, float v) => Drop(u) * Drop(v);

    public static float Noise(float x, float y, int[] permutation, Vector2[] gradients)
    {
        var cell = new Vector2((float)Math.Floor(x), (float)Math.Floor(y));

        var total = 0f;

        var corners = new[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1) };

        foreach (var n in corners)
        {
            var ij = cell + n;
            var uv = new Vector2(x - ij.X, y - ij.Y);

            var index = permutation[(int)ij.X % permutation.Length];
            index = permutation[(index + (int)ij.Y) % permutation.Length];

            var grad = gradients[index % gradients.Length];

            total += Q(uv.X, uv.Y) * Vector2.Dot(grad, uv);
        }

        return Math.Max(Math.Min(total, 1f), -1f);
    }
}
