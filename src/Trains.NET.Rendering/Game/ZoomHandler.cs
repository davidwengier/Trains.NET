namespace Trains.NET.Rendering;

public class ZoomHandler : IInteractionHandler
{
    private const float ZoomInDelta = 1.25f;
    private const float ZoomOutDelta = 1f / ZoomInCommand.ZoomInDelta;

    private readonly IPixelMapper _pixelMapper;

    public ZoomHandler(IPixelMapper pixelMapper)
    {
        _pixelMapper = pixelMapper;
    }

    public bool PreHandleNextClick => false;

    public bool HandlePointerAction(int x, int y, int width, int height, PointerAction action)
    {
        bool didAdjust = false;
        if (action == PointerAction.ZoomIn)
        {
            didAdjust = _pixelMapper.AdjustGameScale(ZoomInDelta);
        }
        else if (action == PointerAction.ZoomOut)
        {
            didAdjust = _pixelMapper.AdjustGameScale(ZoomOutDelta);
        }

        if (didAdjust == false)
        {
            return false;
        }

        // TODO: Move the viewport so its centered on where the mouse pointer is

        return true;
    }
}
