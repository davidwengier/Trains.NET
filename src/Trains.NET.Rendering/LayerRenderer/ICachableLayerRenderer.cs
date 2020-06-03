namespace Trains.NET.Rendering.LayerRenderer
{
    public interface ICachableLayerRenderer
    {
        bool IsDirty { get; }
    }
}
