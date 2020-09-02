using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    [Order(450)]
    public class NatureLayoutRenderer : CachableLayoutRenderer<Tree>
    {
        public NatureLayoutRenderer(ILayout<Tree> layout, IRenderer<Tree> renderer, IImageFactory imageFactory, ITerrainMap terrainMap)
                : base(layout, renderer, imageFactory, terrainMap)
        {
            this.Name = "Nature";
            this.Enabled = true;
            this.IsScaledByHeight = true;
        }
    }
}
