namespace Trains.NET.Rendering.Particles
{
    public class FixedSizeParticleSystem : IParticleSystem
    {
        private const int SystemSize = 1000;
        private readonly Particle[] _particles = new Particle[SystemSize];
        private int _currentIndex = SystemSize - 1;
        private int _lastIndex = SystemSize - 1;

        public void Emit(ParticleType type, float lifetime)
        {
            // Need to decide what to expose
            ParticleState start = new ParticleState();
            ParticleState end = new ParticleState();
            _particles[--_currentIndex].Replace(start, end, type, lifetime);
        }

        public void Draw(ICanvas canvas)
        {
            for (int i = _currentIndex; i <= _lastIndex; i++)
                _particles[i].Draw(canvas);
        }

        public void Update(float delta)
        {
            int lastIndex = _currentIndex;
            for (int i = _currentIndex; i <= _lastIndex; i++)
            {
                _particles[i].Update(delta);
                if(_particles[i].IsAlive)
                {
                    lastIndex = i;
                }
            }
            _lastIndex = lastIndex;
        }
    }
}
