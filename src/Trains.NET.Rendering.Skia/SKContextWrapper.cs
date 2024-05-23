using SkiaSharp;

namespace Trains.NET.Rendering.Skia;

public class SKContextWrapper(GRContext context) : IContext
{
    public GRContext Context { get; } = context;
}
