namespace Trains.NET.Rendering.Particles
{
    internal struct ParticleColor
    {
        public float A { get; private set; }
        public float R { get; private set; }
        public float G { get; private set; }
        public float B { get; private set; }

        public void Replace(float a, float r, float g, float b)
        {
            this.A = a;
            this.R = r;
            this.G = g;
            this.B = b;
        }
    }
}
