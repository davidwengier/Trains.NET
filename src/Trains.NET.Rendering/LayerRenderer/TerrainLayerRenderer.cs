using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    [Order(0)]
    // Terrain layer has its own efficient caching, so doesn't need to be an ICachedLayerRenderer
    public class TerrainLayerRenderer : ILayerRenderer
    {
        private readonly ITerrainMapRenderer _terrainMapRenderer;

        public TerrainLayerRenderer(ITerrainMapRenderer terrainMapRenderer)
        {
            _terrainMapRenderer = terrainMapRenderer;
        }

        public bool Enabled { get; set; } = true;
        public string Name => "Terrain";

        public void Render(ICanvas canvas, int width, int height, IPixelMapper pixelMapper)
        {
            if(_terrainMapRenderer.TryGetTerrainImage(out IImage? terrainImage))
            {
                (Rectangle source, Rectangle destination) = GetSourceAndDestinationRectangles(pixelMapper);

                canvas.DrawImage(terrainImage, source, destination);
            }
        }

        private static (Rectangle Source, Rectangle Destination) GetSourceAndDestinationRectangles(IPixelMapper pixelMapper)
        {
            (int topLeftColumn, int topLeftRow) = pixelMapper.ViewPortPixelsToCoords(0, 0);
            (int bottomRightColumn, int bottomRightRow) = pixelMapper.ViewPortPixelsToCoords(pixelMapper.ViewPortWidth, pixelMapper.ViewPortHeight);

            bottomRightColumn += 1;
            bottomRightRow += 1;

            Rectangle source = new(topLeftColumn, topLeftRow, bottomRightColumn, bottomRightRow);

            (int destinationTopLeftX, int destinationTopLeftY, _) = pixelMapper.CoordsToViewPortPixels(topLeftColumn, topLeftRow);
            (int destinationBottomRightX, int destinationBottomRightY, _) = pixelMapper.CoordsToViewPortPixels(bottomRightColumn, bottomRightRow);

            Rectangle destination = new(destinationTopLeftX, destinationTopLeftY, destinationBottomRightX, destinationBottomRightY);

            return (source, destination);
        }
    }
}
