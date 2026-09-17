using Trains.NET.Rendering.Skia;

namespace Trains.NET.Tests.Rendering;

public class SKImageFactoryTests
{
    [Fact]
    public void CreateImageCanvasUsesDisplayScale()
    {
        var factory = new SKImageFactory();
        factory.SetDisplayScale(1.5f, 2);

        using var imageCanvas = factory.CreateImageCanvas(40, 20);
        using var image = Assert.IsType<SKImageWrapper>(imageCanvas.Render());

        Assert.Equal(60, image.Image.Width);
        Assert.Equal(40, image.Image.Height);
        Assert.Equal(40, image.LogicalWidth);
        Assert.Equal(20, image.LogicalHeight);
        Assert.Equal(1.5f, image.ScaleX);
        Assert.Equal(2, image.ScaleY);
    }
}
