using Trains.NET.Rendering;

namespace Trains.NET.Tests.Rendering.PixelMapperTests;

public class AdjustGameScaleTests(ITestOutputHelper output) : IAsyncLifetime
{
    private const int ScreenSize = 720;

    private readonly PixelMapper _pixelMapper = new PixelMapper();
    private readonly ITestOutputHelper _output = output;

    public async Task InitializeAsync()
    {
        await _pixelMapper.InitializeAsync(100, 100);

        _pixelMapper.SetViewPortSize(ScreenSize, ScreenSize);
        int centerViewportOffsetX = _pixelMapper.MaxGridWidth / 2 - ScreenSize / 2;
        int centerViewportOffsetY = _pixelMapper.MaxGridHeight / 2 - ScreenSize / 2;

        _pixelMapper.SetViewPort(centerViewportOffsetX, centerViewportOffsetY);
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    [Fact]
    public async Task AdjustGameScale_CantZoomToZero()
    {
        const int ScreenSize = 200;
        var pixelMapper = new PixelMapper();
        await pixelMapper.InitializeAsync(100, 100);

        pixelMapper.SetViewPortSize(ScreenSize, ScreenSize);

        pixelMapper.AdjustGameScale(0.0f);
        pixelMapper.LogData(_output);

        Assert.Equal(1.0f, pixelMapper.GameScale);
    }

    [Fact]
    public async Task AdjustGameScale_SingleZoomOut_CorrectViewportWorldPosition()
    {
        const int ScreenSize = 200;
        var pixelMapper = new PixelMapper();
        await pixelMapper.InitializeAsync(100, 100);

        pixelMapper.SetViewPortSize(ScreenSize, ScreenSize);

        // Set the inital viewport to be at 100,100
        pixelMapper.SetViewPort(100, 100);
        pixelMapper.LogData(_output);

        Assert.Equal(100, -pixelMapper.ViewPortX);
        Assert.Equal(100, -pixelMapper.ViewPortY);

        pixelMapper.AdjustGameScale(0.5f);
        pixelMapper.LogData(_output);

        Assert.Equal(0, -pixelMapper.ViewPortX);
        Assert.Equal(0, -pixelMapper.ViewPortY);
    }

    [Fact]
    public async Task AdjustGameScale_SingleZoomIn_CorrectViewportWorldPosition()
    {
        const int ScreenSize = 200;
        var pixelMapper = new PixelMapper();
        await pixelMapper.InitializeAsync(100, 100);

        pixelMapper.SetViewPortSize(ScreenSize, ScreenSize);

        // Set the inital viewport to be at 100,100
        pixelMapper.SetViewPort(100, 100);
        pixelMapper.LogData(_output);

        Assert.Equal(100, -pixelMapper.ViewPortX);
        Assert.Equal(100, -pixelMapper.ViewPortY);

        pixelMapper.AdjustGameScale(2f);
        pixelMapper.LogData(_output);

        Assert.Equal(300, -pixelMapper.ViewPortX);
        Assert.Equal(300, -pixelMapper.ViewPortY);
    }

    [Fact]
    public async Task AdjustGameScale_DoubleZoomIn_CorrectViewportWorldPosition()
    {
        const int ScreenSize = 200;
        var pixelMapper = new PixelMapper();
        await pixelMapper.InitializeAsync(100, 100);

        pixelMapper.SetViewPortSize(ScreenSize, ScreenSize);

        // Set the inital viewport to be at 100,100
        pixelMapper.SetViewPort(100, 100);
        pixelMapper.LogData(_output);

        Assert.Equal(100, -pixelMapper.ViewPortX);
        Assert.Equal(100, -pixelMapper.ViewPortY);

        pixelMapper.AdjustGameScale(2f);
        pixelMapper.LogData(_output);

        Assert.Equal(300, -pixelMapper.ViewPortX);
        Assert.Equal(300, -pixelMapper.ViewPortY);

        pixelMapper.AdjustGameScale(2f);
        pixelMapper.LogData(_output);

        Assert.Equal(700, -pixelMapper.ViewPortX);
        Assert.Equal(700, -pixelMapper.ViewPortY);
    }

    [Fact]
    public async Task AdjustGameScale_DoubleZoomIn_OddPos_CorrectViewportWorldPosition()
    {
        const int ScreenSize = 200;
        var pixelMapper = new PixelMapper();
        await pixelMapper.InitializeAsync(100, 100);

        pixelMapper.SetViewPortSize(ScreenSize, ScreenSize);

        // Set the inital viewport to be at 100,100
        pixelMapper.SetViewPort(350, 700);
        pixelMapper.LogData(_output);

        Assert.Equal(350, -pixelMapper.ViewPortX);
        Assert.Equal(700, -pixelMapper.ViewPortY);

        pixelMapper.AdjustGameScale(2f);
        pixelMapper.LogData(_output);

        Assert.Equal(800, -pixelMapper.ViewPortX);
        Assert.Equal(1500, -pixelMapper.ViewPortY);

        pixelMapper.AdjustGameScale(2f);
        pixelMapper.LogData(_output);

        Assert.Equal(1700, -pixelMapper.ViewPortX);
        Assert.Equal(3100, -pixelMapper.ViewPortY);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void AdjustGameScale_ZoomIn_SameCell(int zoomSteps)
    {
        (int origCol, int origRow) = _pixelMapper.GetMiddleCoordsOfViewPort();
        _pixelMapper.LogData(_output);

        for (int i = 0; i < zoomSteps; i++)
        {
            _pixelMapper.ZoomInPixelMapper();
            _pixelMapper.LogData(_output);
        }

        (int newCol, int newRow) = _pixelMapper.GetMiddleCoordsOfViewPort();

        Assert.Equal(origCol, newCol);
        Assert.Equal(origRow, newRow);
    }

    [Theory]
    [InlineData(1)]
    public void AdjustGameScale_ZoomOut_SameCell(int zoomSteps)
    {
        (int origCol, int origRow) = _pixelMapper.GetMiddleCoordsOfViewPort();
        _pixelMapper.LogData(_output);

        for (int i = 0; i < zoomSteps; i++)
        {
            _pixelMapper.ZoomOutPixelMapper();
            _pixelMapper.LogData(_output);
        }

        (int newCol, int newRow) = _pixelMapper.GetMiddleCoordsOfViewPort();

        Assert.Equal(origCol, newCol);
        Assert.Equal(origRow, newRow);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void AdjustGameScale_ZoomInZoomOut_SameCell(int zoomSteps)
    {
        (int origCol, int origRow) = _pixelMapper.GetMiddleCoordsOfViewPort();
        _pixelMapper.LogData(_output);

        for (int i = 0; i < zoomSteps; i++)
        {
            _pixelMapper.ZoomInPixelMapper();
            _pixelMapper.LogData(_output);
        }
        for (int i = 0; i < zoomSteps; i++)
        {
            _pixelMapper.ZoomOutPixelMapper();
            _pixelMapper.LogData(_output);
        }

        (int newCol, int newRow) = _pixelMapper.GetMiddleCoordsOfViewPort();

        Assert.Equal(origCol, newCol);
        Assert.Equal(origRow, newRow);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void AdjustGameScale_ZoomOutZoomIn_SameCell(int zoomSteps)
    {
        (int origCol, int origRow) = _pixelMapper.GetMiddleCoordsOfViewPort();
        _pixelMapper.LogData(_output);

        for (int i = 0; i < zoomSteps; i++)
        {
            _pixelMapper.ZoomOutPixelMapper();
            _pixelMapper.LogData(_output);
        }
        for (int i = 0; i < zoomSteps; i++)
        {
            _pixelMapper.ZoomInPixelMapper();
            _pixelMapper.LogData(_output);
        }

        (int newCol, int newRow) = _pixelMapper.GetMiddleCoordsOfViewPort();

        Assert.Equal(origCol, newCol);
        Assert.Equal(origRow, newRow);
    }
}
