using System.Collections.Generic;
using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    [Order(450)]
    public class TrackLayoutRenderer : StaticEntityCollectionRenderer<Track>
    {
        public TrackLayoutRenderer(ILayout<Track> layout, IEnumerable<IStaticEntityRenderer<Track>> renderers, IImageFactory imageFactory, IImageCache imageCache)
                : base(layout, renderers, imageFactory, imageCache)
        {
            this.Name = "Tracks";
        }
    }
}
