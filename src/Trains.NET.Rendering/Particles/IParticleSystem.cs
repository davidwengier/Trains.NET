namespace Trains.NET.Rendering.Particles
{
    public interface IParticleSystem
    {
        void Update(float delta);
        void Draw(ICanvas canvas);
        void Emit(ParticleType type, float lifetime);
    }
}
