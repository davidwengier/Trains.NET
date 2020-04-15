namespace Trains.NET.Rendering.HtmlCanvas
{
    internal class PathFactory : IPathFactory
    {
        public IPath Create()
        {
            return new PathWrapper();
        }
    }
}
