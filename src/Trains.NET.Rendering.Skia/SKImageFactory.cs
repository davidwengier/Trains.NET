using SkiaSharp;

namespace Trains.NET.Rendering.Skia
{
    public class SKImageFactory : IImageFactory
    {
        private GRContext? _context;

        public IImageCanvas CreateImageCanvas(int width, int height)
        {
            return new SKSurfaceWrapper(width, height, _context);
        }

        public void SetContext(GRContext? context)
        {
            _context = context;
        }
    }
}
