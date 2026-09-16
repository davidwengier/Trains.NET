using Trains.NET.Rendering;
using Trains.NET.Rendering.UI;

namespace Trains.NET.Tests.Rendering;

public class ButtonBaseTests
{
    [Fact]
    public void MovingOverButtonShowsTooltip()
    {
        var tooltipService = new TestTooltipService();
        var button = new TestButton(tooltipService, "Tooltip")
        {
            Width = 40,
            Height = 40
        };

        button.HandleMouseAction(20, 20, PointerAction.Move);

        Assert.Equal("Tooltip", tooltipService.Text);
    }

    [Fact]
    public void ClickingButtonDoesNotShowTooltip()
    {
        var tooltipService = new TestTooltipService();
        var button = new TestButton(tooltipService, "Tooltip")
        {
            Width = 40,
            Height = 40
        };

        button.HandleMouseAction(20, 20, PointerAction.Click);

        Assert.Null(tooltipService.Text);
    }

    private sealed class TestButton(ITooltipService tooltipService, string tooltip)
        : ButtonBase(() => false, () => { }, tooltipService, tooltip)
    {
        public override int GetMinimumWidth(ICanvas canvas) => 40;

        protected override void RenderButtonLabel(ICanvas canvas)
        {
        }
    }

    private sealed class TestTooltipService : ITooltipService
    {
        public string? Text { get; private set; }

        public void PointerMoved(int x, int y)
        {
            Text = null;
        }

        public void Show(string text)
        {
            Text = text;
        }

        public void Hide()
        {
            Text = null;
        }
    }
}
