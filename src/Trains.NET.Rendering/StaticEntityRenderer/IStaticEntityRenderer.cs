using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    public interface IStaticEntityRenderer<T> : IRenderer<T> where T : IStaticEntity
    {
    }
}
