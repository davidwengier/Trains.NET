using Trains.NET.Instrumentation;
using Trains.NET.Rendering;
using Trains.NET.Rendering.Skia;

namespace WinTrains;

public partial class MainForm : Form
{
    private const float LogicalDpi = 96;

    private readonly PerSecondTimedStat _fps = InstrumentationBag.Add<PerSecondTimedStat>("SKControl-FPS");
    private readonly ElapsedMillisecondsTimedStat _drawTime = InstrumentationBag.Add<ElapsedMillisecondsTimedStat>("SKControl-DrawTime");

    private readonly IGame _game;
    private readonly IInteractionManager _interactionManager;
    private bool _presenting = true;

    public MainForm()
    {
        InitializeComponent();

        Text = "Trains - @davidwengier@aus.social - " + ThisAssembly.AssemblyInformationalVersion;

        _game = DI.ServiceLocator.GetService<IGame>();
        _interactionManager = DI.ServiceLocator.GetService<IInteractionManager>();

        _game.InitializeAsync(200, 200).GetAwaiter().GetResult();

        UpdateGameSize();

        _skControl.SizeChanged += (s, e) => UpdateGameSize();
        DpiChanged += (s, e) => UpdateGameSize();
        _skControl.MouseDown += SKControl_MouseDown;
        _skControl.MouseMove += SKControl_MouseMove;
        _skControl.MouseUp += SKControl_MouseUp;
        _skControl.MouseWheel += SKControl_MouseWheel;

        _ = PresentLoop();
    }

    private void SKControl_MouseWheel(object? sender, MouseEventArgs e)
    {
        (var x, var y) = ToGameCoordinates(e.X, e.Y);

        if (e.Delta > 0)
        {
            _interactionManager.PointerZoomIn(x, y);
        }
        else
        {
            _interactionManager.PointerZoomOut(x, y);
        }
    }

    private void SKControl_MouseDown(object? sender, MouseEventArgs e)
    {
        (var x, var y) = ToGameCoordinates(e.X, e.Y);

        if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
        {
            _interactionManager.PointerClick(x, y);
        }
        else if ((e.Button & MouseButtons.Right) == MouseButtons.Right)
        {
            _interactionManager.PointerAlternateClick(x, y);
        }
    }

    private void SKControl_MouseMove(object? sender, MouseEventArgs e)
    {
        (var x, var y) = ToGameCoordinates(e.X, e.Y);

        if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
        {
            _interactionManager.PointerDrag(x, y);
        }
        else if ((e.Button & MouseButtons.Right) == MouseButtons.Right)
        {
            _interactionManager.PointerAlternateDrag(x, y);
        }
        else
        {
            _interactionManager.PointerMove(x, y);
        }
    }

    private void SKControl_MouseUp(object? sender, MouseEventArgs e)
    {
        if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
        {
            (var x, var y) = ToGameCoordinates(e.X, e.Y);
            _interactionManager.PointerRelease(x, y);
        }
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        base.OnFormClosing(e);

        if (e.Cancel)
        {
            return;
        }

        _presenting = false;
        _game.Dispose();
    }

    private void SKControl_PaintSurface(object sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e)
    {
        var scale = GetDisplayScale();
        _game.SetDisplayScale(scale, scale);

        using var canvas = new SKCanvasWrapper(e.Surface.Canvas);
        canvas.Scale(scale, scale);
        _game.Render(canvas);
    }

    private void UpdateGameSize()
    {
        var scale = GetDisplayScale();
        _game.SetDisplayScale(scale, scale);
        _game.SetSize(ToLogicalPixels(_skControl.Width, scale), ToLogicalPixels(_skControl.Height, scale));
    }

    private (int X, int Y) ToGameCoordinates(int x, int y)
    {
        var scale = GetDisplayScale();
        return (ToLogicalPixels(x, scale), ToLogicalPixels(y, scale));
    }

    private float GetDisplayScale() => _skControl.DeviceDpi / LogicalDpi;

    private static int ToLogicalPixels(int pixels, float scale)
        => (int)Math.Round(pixels / scale);

    private async Task PresentLoop()
    {
        while (_presenting)
        {
            using (_drawTime.Measure())
            {
                _skControl.Invalidate();
            }

            _drawTime.Stop();

            _fps.Update();

            await Task.Delay(16).ConfigureAwait(true);
        }
    }
}
