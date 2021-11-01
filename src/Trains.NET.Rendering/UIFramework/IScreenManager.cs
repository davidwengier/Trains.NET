namespace Trains.NET.Rendering;

public interface IScreenManager
{
    /// <summary>
    /// Called when the actual screen is being repainted
    /// </summary>
    void Render(ICanvas canvas);
}
