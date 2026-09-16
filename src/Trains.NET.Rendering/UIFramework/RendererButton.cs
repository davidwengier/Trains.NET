namespace Trains.NET.Rendering.UI;

public class RendererButton<T>(
    T entity,
    IRenderer<T> renderer,
    Func<bool> isActive,
    Action onClick) : ButtonBase(isActive, onClick)
{
    private const int ButtonSize = 40;
    private const int RenderSize = 32;
    private const int CanvasSize = 100;

    private readonly T _entity = entity;
    private readonly IRenderer<T> _renderer = renderer;

    public override int GetMinimumWidth(ICanvas canvas) => ButtonSize;

    protected override void RenderButtonLabel(ICanvas canvas)
    {
        if (!_renderer.ShouldRender(_entity))
        {
            return;
        }

        using (canvas.Scope())
        {
            canvas.Translate((Width - RenderSize) / 2, (Height - RenderSize) / 2);
            canvas.Scale((float)RenderSize / CanvasSize, (float)RenderSize / CanvasSize);
            _renderer.Render(canvas, _entity);
        }
    }
}
