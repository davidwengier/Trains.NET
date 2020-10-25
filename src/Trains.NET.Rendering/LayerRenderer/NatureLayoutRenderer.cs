using System.Collections.Generic;
using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    [Order(450)]
    public class NatureLayoutRenderer : StaticEntityCollectionRenderer<Tree>
    {
        public NatureLayoutRenderer(ILayout<Tree> layout, IEnumerable<IStaticEntityRenderer<Tree>> renderers, IImageFactory imageFactory, IImageCache imageCache)
                : base(layout, renderers, imageFactory, imageCache)
        {
            this.Name = "Nature";
            this.IsScaledByHeight = true;
        }
    }
}
