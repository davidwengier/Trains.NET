namespace Trains.NET.Rendering
{
    public interface IRenderer<T>
    {
        void Render(ICanvas canvas, T entity);
        bool ShouldRender(T entity);
    }
}
