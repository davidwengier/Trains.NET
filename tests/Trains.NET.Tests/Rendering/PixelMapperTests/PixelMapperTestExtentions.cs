using Trains.NET.Rendering;

namespace Trains.NET.Tests.Rendering;

public static class PixelMapperTestExtentions
{
    internal const float ZoomInDelta = 1.0f / ZoomOutDelta;
    internal const float ZoomOutDelta = 0.8f;

    public static (int Col, int Row) GetMiddleCoordsOfViewPort(this IPixelMapper pixelMapper)
        => pixelMapper.ViewPortPixelsToCoords(pixelMapper.ViewPortWidth / 2, pixelMapper.ViewPortHeight / 2);

    public static void ZoomInPixelMapper(this IPixelMapper pixelMapper) => pixelMapper.AdjustGameScale(ZoomInDelta);

    public static void ZoomOutPixelMapper(this IPixelMapper pixelMapper) => pixelMapper.AdjustGameScale(ZoomOutDelta);

    public static void LogData(this IPixelMapper pixelMapper, ITestOutputHelper output)
    {
        output.WriteLine("-----");
        output.WriteLine("Game Scale: " + pixelMapper.GameScale);
        output.WriteLine("Cell Size: " + pixelMapper.CellSize);
        output.WriteLine("Viewport X: " + pixelMapper.ViewPortX);
        output.WriteLine("Viewport Y: " + pixelMapper.ViewPortY);
        output.WriteLine("Viewport Width: " + pixelMapper.ViewPortWidth);
        output.WriteLine("Viewport Height: " + pixelMapper.ViewPortHeight);
        (int col, int row) = GetMiddleCoordsOfViewPort(pixelMapper);
        output.WriteLine("Viewport Center Column: " + col);
        output.WriteLine("Viewport Center Row: " + row);
    }
}
