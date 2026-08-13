using Trains.NET.Rendering;
using Trains.NET.Rendering.UI;

namespace Trains.NET.Tests.Rendering;

public class MultiButtonTests
{
    [Fact]
    public void MovingToEarlierButtonClearsLaterButtonHover()
    {
        var first = new TrackingButton();
        var second = new TrackingButton();
        var multiButton = new MultiButton(40, first, second);

        multiButton.HandleMouseAction(60, 20, PointerAction.Move);
        Assert.True(second.IsHovered);

        multiButton.HandleMouseAction(20, 20, PointerAction.Move);

        Assert.True(first.IsHovered);
        Assert.False(second.IsHovered);
    }

    private sealed class TrackingButton : ButtonBase
    {
        public TrackingButton()
        {
            Width = 40;
            Height = 40;
        }

        public bool IsHovered { get; private set; }

        public override bool HandleMouseAction(int x, int y, PointerAction action)
        {
            IsHovered = x is >= 0 and <= 40 && y is >= 0 and <= 40;
            return base.HandleMouseAction(x, y, action);
        }

        public override int GetMinimumWidth(ICanvas canvas) => 40;

        protected override void RenderButtonLabel(ICanvas canvas)
        {
        }
    }
}
