using System;

namespace Trains.NET.Rendering
{
    public interface IScreen
    {
        event EventHandler? Changed;

        void Render(ICanvas canvas, int width, int height);

        bool HandleInteraction(int x, int y, int width, int height, MouseAction action);
    }
}
