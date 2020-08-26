namespace Trains.NET.Rendering
{
    public interface IScreen
    {
        void Render(ICanvas canvas, int width, int height);

        bool HandleInteraction(int x, int y, bool pressed);
    }
}
