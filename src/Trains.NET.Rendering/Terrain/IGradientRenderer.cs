namespace Trains.NET.Rendering.Terrain
{
    public interface IGradientRenderer
    {
        public void Render(ICanvas canvas, TerrainCellGradient gradient);
    }
}
