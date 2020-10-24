namespace Trains.NET.Rendering
{
    public interface IInteractionManager
    {
        bool PointerClick(int x, int y);
        bool PointerMove(int x, int y);
        bool PointerDrag(int x, int y);
        bool PointerRelease(int x, int y);
        bool PointerZoomIn(int x, int y);
        bool PointerZoomOut(int x, int y);
    }
}
