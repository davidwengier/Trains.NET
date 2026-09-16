namespace Trains.NET.Rendering.UI;

public interface ITooltipService
{
    void PointerMoved(int x, int y);

    void Show(string text);

    void Hide();
}
