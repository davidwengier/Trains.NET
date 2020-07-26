namespace Trains.NET.Rendering.Skia
{
    internal class SKImageFactory : IImageFactory
    {
        public IImageCanvas CreateImageCanvas(int width, int height)
        {
            return new SKSurfaceWrapper(width, height);
        }
    }
}
