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
        float GameScale { get; }
        int CellSize { get; }
        int Rows { get; }
        int Columns { get; }
        int MaxGridWidth { get; }
        int MaxGridHeight { get; }

        (int X, int Y) CoordsToWorldPixels(int column, int row);
        (int Column, int Row) WorldPixelsToCoords(int x, int y);
        (int X, int Y, bool OnScreen) CoordsToViewPortPixels(int column, int row);
        (int Column, int Row) ViewPortPixelsToCoords(int x, int y);
        void AdjustViewPort(int x, int y);
        void SetViewPortSize(int width, int height);
        void SetViewPort(int x, int y);
        IPixelMapper Snapshot();
        bool AdjustGameScale(float delta);
        (int X, int Y) WorldPixelsToViewPortPixels(int x, int y);
    }
}
