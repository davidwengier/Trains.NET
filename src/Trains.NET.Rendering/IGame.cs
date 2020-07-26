
using System;

namespace Trains.NET.Rendering
{
    public interface IGame : IDisposable
    {
        void AdjustViewPortIfNecessary();
        void Render(ICanvas canvas);
        void SetSize(int width, int height);
    }
}
