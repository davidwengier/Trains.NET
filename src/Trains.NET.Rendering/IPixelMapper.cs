using System;

namespace Trains.NET.Rendering
{
    public interface IPixelMapper
    {
        event EventHandler? ViewPortChanged;

        int ViewPortX { get; }
        int ViewPortY { get; }
        int ViewPortWidth { get; }
        int ViewPortHeight { get; }
        float GameScale { get; set; }
        int CellSize { get; }

        (int X, int Y) CoordsToWorldPixels(int column, int row);
        (int Column, int Row) WorldPixelsToCoords(int x, int y);
        (int X, int Y, bool OnScreen) CoordsToViewPortPixels(int column, int row);
        (int Column, int Row) ViewPortPixelsToCoords(int x, int y);
        void AdjustViewPort(int x, int y);
        void SetViewPortSize(int width, int height);
        void SetViewPort(int x, int y);
        IPixelMapper Snapshot();
    }
}
