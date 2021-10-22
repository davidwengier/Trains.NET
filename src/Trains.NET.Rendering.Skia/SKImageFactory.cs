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

        public bool SetContext(IContext context)
        {
            if (context is SKContextWrapper skContext)
            {
                bool initalSet = _context == null;
                _context = skContext.Context;
                return initalSet;
            }
            return false;
        }
    }
}
