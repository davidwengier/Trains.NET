
namespace Trains.NET.Rendering
{
    public interface IGame
    {
        void Render(ICanvas canvas);
        void SetSize(int width, int height);
    }
}
