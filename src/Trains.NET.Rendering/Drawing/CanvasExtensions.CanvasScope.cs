using System;

namespace Trains.NET.Rendering
{
    public static partial class CanvasExtensions
    {
        private class CanvasScope : IDisposable
        {
            private readonly ICanvas _canvas;

            public CanvasScope(ICanvas canvas)
            {
                _canvas = canvas;
                _canvas.Save();
            }

            public void Dispose()
            {
                _canvas.Restore();
            }
        }
    }
}
