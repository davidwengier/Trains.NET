namespace Trains.NET.Rendering.LayerRenderer
{
    public interface ICachableLayerRenderer : ILayerRenderer
    {
        bool IsDirty { get; }
    }
}
