namespace Trains.NET.Rendering
{
    public interface IInteractionHandler
    {
        bool HandlePointerAction(int x, int y, int width, int height, PointerAction action);
    }
}