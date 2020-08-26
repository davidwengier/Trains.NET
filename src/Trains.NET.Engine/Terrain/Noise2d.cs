using System;
using System.Linq;
using System.Numerics;

namespace Trains.NET.Engine
{
    public static class Noise2d
    {
        private static readonly Random s_random = new Random();
        private static int[] s_permutation;

        private static readonly Vector2[] s_gradients;

        static Noise2d()
        {
            CalculatePermutation(out s_permutation);
            CalculateGradients(out s_gradients);
        }

        private static void CalculatePermutation(out int[] p)
        {
            p = Enumerable.Range(0, 256).ToArray();

            /// shuffle the array
            for (int i = 0; i < p.Length; i++)
            {
                int source = s_random.Next(p.Length);

                int t = p[i];
                p[i] = p[source];
                p[source] = t;
            }
        }

        /// <summary>
        /// generate a new permutation.
        /// </summary>
        public static void Reseed()
        {
            CalculatePermutation(out s_permutation);
        }

        private static void CalculateGradients(out Vector2[] grad)
        {
            grad = new Vector2[256];

            for (int i = 0; i < grad.Length; i++)
            {
                Vector2 gradient;

                do
                {
                    gradient = new Vector2((float)(s_random.NextDouble() * 2 - 1), (float)(s_random.NextDouble() * 2 - 1));
                }
                while (gradient.LengthSquared() >= 1);

                gradient = Vector2.Normalize(gradient);

                grad[i] = gradient;
            }

        }

        private static float Drop(float t)
        {
            t = Math.Abs(t);
            return 1f - t * t * t * (t * (t * 6 - 15) + 10);
        }

        private static float Q(float u, float v)
        {
            return Drop(u) * Drop(v);
        }

        public static float Noise(float x, float y)
        {
            var cell = new Vector2((float)Math.Floor(x), (float)Math.Floor(y));

            float total = 0f;

            Vector2[]? corners = new[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1) };

            foreach (Vector2 n in corners)
            {
                Vector2 ij = cell + n;
                var uv = new Vector2(x - ij.X, y - ij.Y);

                int index = s_permutation[(int)ij.X % s_permutation.Length];
                index = s_permutation[(index + (int)ij.Y) % s_permutation.Length];

                Vector2 grad = s_gradients[index % s_gradients.Length];

                total += Q(uv.X, uv.Y) * Vector2.Dot(grad, uv);
            }

            return Math.Max(Math.Min(total, 1f), -1f);
        }

    }
}
