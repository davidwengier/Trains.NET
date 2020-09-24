using System;

namespace Trains.NET.Rendering
{
    public interface ISwapChain
    {
        void DrawNext(Action<ICanvas> draw);
    }
}
