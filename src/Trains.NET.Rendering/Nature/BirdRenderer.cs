using System;
using System.Collections.Generic;
using System.Numerics;
using Trains.NET.Engine;

namespace Trains.NET.Rendering.Nature
{
    [Order(9000)]
    public class BirdRenderer : ILayerRenderer
    {
#pragma warning disable IDE1006 // Naming Styles
        private record Bird(Vector2 Start, Vector2 End, Vector2 Control, float Steps, int Size)
        {
            public Vector2 Location { get; set; }
            public float Count { get; set; }
            public double Angle { get; internal set; }
        }
#pragma warning restore IDE1006 // Naming Styles

        private readonly List<Bird> _birds = new List<Bird>();
        private readonly Random _random = new Random();

        private readonly PaintBrush _paint = new PaintBrush
        {
            Style = PaintStyle.Fill,
            Color = Colors.Black
        };

        public bool Enabled { get; set; } = true;

        public string Name => "Birds";

        public void Render(ICanvas canvas, int width, int height, IPixelMapper pixelMapper)
        {
            if (_birds.Count < 10 && _random.Next(100) < 25)
            {
                _birds.Add(SpawnBird());
            }

            foreach (var bird in _birds)
            {
                MoveBird(bird);
            }

            for (int i = _birds.Count - 1; i >= 0; i--)
            {
                Bird bird = _birds[i];
                if (bird.Location.X < 0 || bird.Location.X > pixelMapper.MaxGridWidth ||
                    bird.Location.Y < 0 || bird.Location.Y > pixelMapper.MaxGridHeight)
                {
                    _birds.RemoveAt(i);
                }
            }

            foreach (var bird in _birds)
            {
                var (x, y) = pixelMapper.WorldPixelsToViewPortPixels((int)bird.Location.X, (int)bird.Location.Y);
                canvas.Save();
                canvas.RotateDegrees((int)bird.Angle, x, y);
                int radius = pixelMapper.CellSize / bird.Size;
                canvas.DrawRect(x - radius, y - radius / 4, radius * 2, radius / 2, _paint);
                if (bird.Count % 0.05f < 0.02)
                {
                    canvas.DrawRect(x - radius / 4, y - radius, radius / 2, radius * 2, _paint);
                }
                canvas.Restore();
            }

            Bird SpawnBird()
            {
                var startSide = _random.Next(4);
                int endSide;
                do
                {
                    endSide = _random.Next(4);
                }
                while (endSide == startSide);
                var start = GetPoint(startSide, pixelMapper);
                var end = GetPoint(endSide, pixelMapper);

                var control = GetPoint(4, pixelMapper);

                var steps = _random.NextFloat(0.0001f, 0.001f);

                return new Bird(start, end, control, steps, _random.Next(2, 8));

                Vector2 GetPoint(int side, IPixelMapper pixelMapper)
                {
                    var x = side switch
                    {
                        0 => 0,
                        1 or 3 => _random.Next(pixelMapper.MaxGridWidth),
                        2 => pixelMapper.MaxGridWidth,
                        4 => _random.Next(pixelMapper.MaxGridWidth), // special for control points
                        _ => throw new ArgumentException("That's the trouble with random...")
                    };
                    var y = side switch
                    {
                        0 or 2 => _random.Next(pixelMapper.MaxGridHeight),
                        1 => 0,
                        3 => pixelMapper.MaxGridHeight,
                        4 => _random.Next(pixelMapper.MaxGridHeight), // special for control points
                        _ => throw new ArgumentException("That's the trouble with random...")
                    };
                    return new Vector2(x, y);
                }
            }

            void MoveBird(Bird bird)
            {
                var lastLocation = bird.Location;
                bird.Count += 1.0f * bird.Steps;

                Vector2 m1 = Vector2.Lerp(bird.Start, bird.Control, bird.Count);
                Vector2 m2 = Vector2.Lerp(bird.Control, bird.End, bird.Count);
                bird.Location = Vector2.Lerp(m1, m2, bird.Count);

                var distance = Vector2.Subtract(lastLocation, bird.Location);
                bird.Angle = TrainMovement.RadToDegree(TrainMovement.PointsToAngle(distance.X, distance.Y));
            }
        }
    }
}
