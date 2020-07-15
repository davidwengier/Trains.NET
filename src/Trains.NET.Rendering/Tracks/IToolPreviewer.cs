namespace Trains.NET.Rendering.Tracks
{
    public interface IToolPreviewer
    {
        void RenderPreview(ICanvas canvas, int column, int row);
    }
}
