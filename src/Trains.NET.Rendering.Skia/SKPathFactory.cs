namespace Trains.NET.Rendering.Skia
{
    public class SKPathFactory : IPathFactory
    {
        public IPath Create()
        {
            return new SKPathWrapper();
        }
    }
}
