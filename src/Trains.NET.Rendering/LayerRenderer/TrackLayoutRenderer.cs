using System.Collections.Generic;
using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    [Order(450)]
    public class TrackLayoutRenderer : CachableLayoutRenderer<Track>
    {
        public TrackLayoutRenderer(ILayout<Track> layout, IEnumerable<IRenderer<Track>> renderers, IImageFactory imageFactory, ITerrainMap terrainMap, IImageCache imageCache)
                : base(layout, renderers, imageFactory, terrainMap, imageCache)
        {
            this.Name = "Tracks";
            this.Enabled = true;
        }
    }
}
