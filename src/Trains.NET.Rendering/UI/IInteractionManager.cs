namespace Trains.NET.Rendering
{
    public interface IInteractionManager
    {
        void PointerDown(int x, int y);

        void PointerMove(int x, int y);

        void PointerUp(int x, int y);
    }
}
