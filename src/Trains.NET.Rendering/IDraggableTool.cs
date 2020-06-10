namespace Trains.NET.Rendering
{
    public interface IDraggableTool
    {
        void StartDrag(int x, int y);
        void ContinueDrag(int x, int y);
    }
}
