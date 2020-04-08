namespace Trains.NET.Rendering.Skia
{
    internal class SKPathFactory : IPathFactory
    {
        public IPath Create()
        {
            return new SKPathWrapper();
        }
    }
}
