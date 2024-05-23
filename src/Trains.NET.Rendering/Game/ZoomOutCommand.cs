using Trains.NET.Engine;

namespace Trains.NET.Rendering;

public class ZoomOutCommand(IPixelMapper pixelMapper) : ICommand
{
    private const float ZoomOutDelta = 1f / ZoomInCommand.ZoomInDelta;

    private readonly IPixelMapper _pixelMapper = pixelMapper;

    public string Name => "Zoom Out";

    public void Execute()
    {
        _pixelMapper.AdjustGameScale(ZoomOutDelta);
    }
}
