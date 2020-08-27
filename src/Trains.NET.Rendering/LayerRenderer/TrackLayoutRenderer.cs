using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    [Order(450)]
    public class TrackLayoutRenderer : CachableLayoutRenderer<Track>
    {
        public TrackLayoutRenderer(ILayout<Track> layout, IRenderer<Track> renderer, IGameParameters gameParameters, IImageFactory imageFactory)
                : base(layout, renderer, gameParameters, imageFactory)
        {
            this.Name = "Tracks";
            this.Enabled = true;
        }
    }
}
