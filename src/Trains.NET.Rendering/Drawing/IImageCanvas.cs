using System;

namespace Trains.NET.Rendering
{
    public interface IImageCanvas : IDisposable
    {
        ICanvas Canvas { get; }

        IImage Render();
    }
}
