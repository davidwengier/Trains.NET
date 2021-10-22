using SkiaSharp;

namespace Trains.NET.Rendering.Skia
{
    public class SKContextWrapper : IContext
    {
        public GRContext Context { get; }

        public SKContextWrapper(GRContext context)
        {
            this.Context = context;
        }
    }
}
