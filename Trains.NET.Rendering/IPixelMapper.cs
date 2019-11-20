namespace Trains.NET.Rendering
{
    public interface IPixelMapper
    {
        (int X, int Y) CoordsToPixels(int column, int row);
        (int Column, int Row) PixelsToCoords(int x, int y);
    }
}
