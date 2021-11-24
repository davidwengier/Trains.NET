namespace Trains.NET.Rendering;

public interface ICachableLayerRenderer : ILayerRenderer
{
    event EventHandler? Changed;
}
