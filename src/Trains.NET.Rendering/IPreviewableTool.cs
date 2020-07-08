namespace Trains.NET.Rendering
{
    public interface IPreviewableTool
    {
        void RenderPreview(ICanvas canvas, int column, int row);
    }
}
