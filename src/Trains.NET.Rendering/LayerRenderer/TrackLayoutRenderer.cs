using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    [Order(450)]
    public class TrackLayoutRenderer : CachableLayoutRenderer<Track>
    {
        public override bool Enabled { get; set; } = true;
        public override string Name => "Tracks";

        public TrackLayoutRenderer(ILayout<Track> layout, IRenderer<Track> renderer, IGameParameters gameParameters, IImageFactory imageFactory)
                : base(layout, renderer, gameParameters, imageFactory)
        {
        }
    }
}
