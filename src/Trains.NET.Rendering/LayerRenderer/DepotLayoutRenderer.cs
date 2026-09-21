using Trains.NET.Engine;

namespace Trains.NET.Rendering;

[Order(475)]
public class DepotLayoutRenderer : StaticEntityCollectionRenderer<Depot>
{
    public DepotLayoutRenderer(
        ILayout<Depot> layout,
        IEnumerable<IStaticEntityRenderer<Depot>> renderers,
        IImageFactory imageFactory,
        IImageCache imageCache)
        : base(layout, renderers, imageFactory, imageCache)
    {
        Name = "Depots";
    }
}
