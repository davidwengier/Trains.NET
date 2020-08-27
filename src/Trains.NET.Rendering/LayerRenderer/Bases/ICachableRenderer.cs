namespace Trains.NET.Rendering
{
    public interface ICachableRenderer<T> : IRenderer<T>
    {
        string GetCacheKey(T item);
    }
}
