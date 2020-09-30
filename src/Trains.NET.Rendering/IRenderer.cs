using System;
using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    public interface IRenderer<T>
    {
        void Render(ICanvas canvas, T item);
        bool ShouldRender(T entity) => true;
    }
}
