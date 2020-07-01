namespace Trains.NET.Rendering.Particles
{
    internal struct Particle
    {
        public ParticleState Start { get; private set; }
        public ParticleState End { get; private set; }
        public ParticleType Type { get; private set; }
        public float InitialLifetime { get; private set; }
        public bool IsAlive { get; private set; }
        private float _currentLifetime;
        public void Replace(ParticleState start, ParticleState end, ParticleType type, float lifetime)
        {
            this.Start = start;
            this.End = end;
            this.Type = type;
            this.InitialLifetime = lifetime;
            _currentLifetime = lifetime;
            this.IsAlive = true;
        }
        public void Draw(ICanvas canvas)
        {
            if (!this.IsAlive) return;
        }
        public void Update(float delta)
        {
            if (!this.IsAlive) return;
            _currentLifetime -= delta;
            if(_currentLifetime <= 0.0f)
            {
                this.IsAlive = false;
                return;
            }
        }
    }
}
