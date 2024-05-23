using Trains.NET.Engine;

namespace Trains.NET.Rendering;

public class ZoomInCommand(IPixelMapper pixelMapper) : ICommand
{
    public const float ZoomInDelta = 1.25f;

    private readonly IPixelMapper _pixelMapper = pixelMapper;

    public string Name => "Zoom In";

    public void Execute()
    {
        _pixelMapper.AdjustGameScale(ZoomInDelta);
    }
}
