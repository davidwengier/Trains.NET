namespace Trains.NET.Rendering
{
    public interface IImageFactory
    {
        IImageCanvas CreateImageCanvas(int width, int height);
    }
}
