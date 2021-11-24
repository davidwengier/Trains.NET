namespace Trains.NET.Rendering;

public interface IGame : IDisposable
{
    void AdjustViewPortIfNecessary();
    void Render(ICanvas canvas);
    void SetSize(int width, int height);
    (int Width, int Height) GetSize();
    (int Width, int Height) GetScreenSize();
    void SetContext(IContext context);

    Task InitializeAsync(int columns, int rows);
}
