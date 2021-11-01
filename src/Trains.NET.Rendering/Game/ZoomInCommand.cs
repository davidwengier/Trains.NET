using Trains.NET.Engine;

namespace Trains.NET.Rendering;

public class ZoomInCommand : ICommand
{
    public const float ZoomInDelta = 1.25f;

    private readonly IPixelMapper _pixelMapper;

    public string Name => "Zoom In";

    public ZoomInCommand(IPixelMapper pixelMapper)
    {
        _pixelMapper = pixelMapper;
    }

    public void Execute()
    {
        _pixelMapper.AdjustGameScale(ZoomInDelta);
    }
}
