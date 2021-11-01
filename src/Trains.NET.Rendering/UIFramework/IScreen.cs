using System;

namespace Trains.NET.Rendering;

public interface IScreen
{
    event EventHandler? Changed;

    void Render(ICanvas canvas, int width, int height);
}
