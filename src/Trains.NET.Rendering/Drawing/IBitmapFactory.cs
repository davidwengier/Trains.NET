namespace Trains.NET.Rendering
{
    public interface IBitmapFactory
    {
        IBitmap CreateBitmap(int width, int height);
        ICanvas CreateCanvas(IBitmap bitmap);
    }
}