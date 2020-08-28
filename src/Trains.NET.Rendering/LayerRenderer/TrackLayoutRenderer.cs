using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    [Order(450)]
    public class TrackLayoutRenderer : CachableLayoutRenderer<Track>
    {
        public TrackLayoutRenderer(ILayout<Track> layout, IRenderer<Track> renderer, IImageFactory imageFactory)
                : base(layout, renderer, imageFactory)
        {
            this.Name = "Tracks";
            this.Enabled = true;
        }
    }
}
