using System.Collections.Generic;
using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    [Order(450)]
    public class TrackLayoutRenderer : CachableLayoutRenderer<Track>
    {
        public TrackLayoutRenderer(ILayout<Track> layout, IEnumerable<IRenderer<Track>> renderers, IImageFactory imageFactory)
                : base(layout, renderers, imageFactory)
        {
            this.Name = "Tracks";
            this.Enabled = true;
        }
    }
}
