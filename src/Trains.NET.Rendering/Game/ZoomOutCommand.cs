using Trains.NET.Engine;

namespace Trains.NET.Rendering;

public class ZoomOutCommand : ICommand
{
    private const float ZoomOutDelta = 1f / ZoomInCommand.ZoomInDelta;

    private readonly IPixelMapper _pixelMapper;

    public string Name => "Zoom Out";

    public ZoomOutCommand(IPixelMapper pixelMapper)
    {
        _pixelMapper = pixelMapper;
    }

    public void Execute()
    {
        _pixelMapper.AdjustGameScale(ZoomOutDelta);
    }
}
