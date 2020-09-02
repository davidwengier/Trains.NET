using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    [Order(450)]
    public class TrackLayoutRenderer : CachableLayoutRenderer<Track>
    {
        public TrackLayoutRenderer(ILayout<Track> layout, IRenderer<Track> renderer, IImageFactory imageFactory, ITerrainMap terrainMap)
                : base(layout, renderer, imageFactory, terrainMap)
        {
            this.Name = "Tracks";
            this.Enabled = true;
        }
    }
}
