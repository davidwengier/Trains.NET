using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    [Order(450)]
    public class NatureLayoutRenderer : CachableLayoutRenderer<Tree>
    {
        public override bool Enabled { get; set; } = true;
        public override string Name => "Nature";

        public NatureLayoutRenderer(ILayout<Tree> layout, IRenderer<Tree> renderer, IGameParameters gameParameters, IImageFactory imageFactory)
                : base(layout, renderer, gameParameters, imageFactory)
        {
        }
    }
}
