using System.Numerics;

namespace Trains.NET.Rendering.Particles
{
    internal struct ParticleState
    {
        public Vector2 Position { get; private set; }
        public float Rotation { get; private set; }
        public float Scale { get; private set; }
        public ParticleColor Color { get; private set; }
        public void Replace(Vector2 position, float rotation, float scale, ParticleColor color)
        {
            this.Position = position;
            this.Rotation = rotation;
            this.Scale = scale;
            this.Color = color;
        }
    }
}
