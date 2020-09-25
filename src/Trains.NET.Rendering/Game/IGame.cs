
using System;

namespace Trains.NET.Rendering
{
    public interface IGame : IDisposable
    {
        void AdjustViewPortIfNecessary();
        void Render(Action<ISwapChain> render);
        void SetSize(int width, int height);
        (int Width, int Height) GetSize();
        (int Width, int Height) GetScreenSize();
    }
}
