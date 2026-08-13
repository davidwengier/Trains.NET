using Trains.NET.Rendering;
using Trains.NET.Rendering.Skia;

namespace Trains.NET.Tests;

public class SkiaPictureTests
{
    [Theory]
    [InlineData(Picture.Left, 192, 512)]
    [InlineData(Picture.Right, 192, 512)]
    [InlineData(Picture.Backward, 512, 512)]
    [InlineData(Picture.Forward, 512, 512)]
    [InlineData(Picture.Eye, 576, 512)]
    [InlineData(Picture.Trash, 448, 512)]
    [InlineData(Picture.Play, 448, 512)]
    [InlineData(Picture.Pause, 448, 512)]
    [InlineData(Picture.Cross, 352, 512)]
    [InlineData(Picture.Tools, 512, 512)]
    [InlineData(Picture.Eraser, 512, 512)]
    [InlineData(Picture.Plus, 448, 512)]
    [InlineData(Picture.Minus, 448, 512)]
    public void HasExpectedBounds(Picture picture, int width, int height)
    {
        var skPicture = picture.ToSkia();
        var bounds = skPicture.CullRect;

        Assert.True(skPicture.ApproximateOperationCount > 0);
        Assert.Equal(0, bounds.Left);
        Assert.Equal(0, bounds.Top);
        Assert.Equal(width, bounds.Width);
        Assert.Equal(height, bounds.Height);
    }
}
