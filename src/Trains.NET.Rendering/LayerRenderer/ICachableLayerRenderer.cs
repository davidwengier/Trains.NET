namespace Trains.NET.Rendering
{
    public interface ICachableLayerRenderer : ILayerRenderer
    {
        bool IsDirty { get; }
    }
}
