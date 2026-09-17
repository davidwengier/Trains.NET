using SkiaSharp;

namespace Trains.NET.Rendering.Skia;

public class SKImageFactory : IImageFactory
{
    private GRContext? _context;
    private float _scaleX = 1;
    private float _scaleY = 1;

    public IImageCanvas CreateImageCanvas(int width, int height)
    {
        return new SKSurfaceWrapper(width, height, _context, _scaleX, _scaleY);
    }

    public bool SetContext(IContext context)
    {
        if (context is SKContextWrapper skContext)
        {
            var initialSet = _context == null;
            _context = skContext.Context;
            return initialSet;
        }

        return false;
    }

    public bool SetDisplayScale(float scaleX, float scaleY)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(scaleX);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(scaleY);

        if (!float.IsFinite(scaleX))
        {
            throw new ArgumentOutOfRangeException(nameof(scaleX));
        }

        if (!float.IsFinite(scaleY))
        {
            throw new ArgumentOutOfRangeException(nameof(scaleY));
        }

        if (_scaleX == scaleX && _scaleY == scaleY)
        {
            return false;
        }

        _scaleX = scaleX;
        _scaleY = scaleY;
        return true;
    }
}
