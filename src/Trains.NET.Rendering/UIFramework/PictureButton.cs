namespace Trains.NET.Rendering.UI;

public class PictureButton(
    Picture picture,
    float pictureSize,
    Func<bool> isActive,
    Action onClick,
    ITooltipService? tooltipService = null,
    string? tooltip = null) : ButtonBase(isActive, onClick, tooltipService, tooltip)
{
    private readonly Picture _picture = picture;
    private readonly float _pictureSize = pictureSize;

    public override int GetMinimumWidth(ICanvas canvas)
        => (int)_pictureSize;

    protected override void RenderButtonLabel(ICanvas canvas)
    {
        canvas.DrawPicture(_picture, (Width - _pictureSize) / 2, (Height - _pictureSize) / 2, _pictureSize);
    }
}
