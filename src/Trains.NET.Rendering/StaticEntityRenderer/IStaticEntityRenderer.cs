namespace Trains.NET.Rendering
{
    public interface IStaticEntityRenderer<T> : IRenderer<T>
    {
        string GetCacheKey(T entity);
    }
}
