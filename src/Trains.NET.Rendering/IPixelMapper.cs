using System;

namespace Trains.NET.Rendering
{
    public interface IPixelMapper
    {
        event EventHandler? ViewPortChanged;

        int ViewPortX { get; }
        int ViewPortY { get; }

        (int X, int Y) CoordsToPixels(int column, int row);
        (int Column, int Row) PixelsToCoords(int x, int y);
        void AdjustViewPort(int x, int y);
        void SetViewPortSize(int width, int height);
    }
}
