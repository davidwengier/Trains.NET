namespace Trains.NET.Rendering;

public interface IInteractionHandler
{
    bool PreHandleNextClick { get; }

    bool HandlePointerAction(int x, int y, int width, int height, PointerAction action);
}
