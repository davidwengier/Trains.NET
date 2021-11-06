using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Forms;
using Trains.NET.Engine;
using Trains.NET.Instrumentation;
using Trains.NET.Rendering;
using Trains.NET.Rendering.Skia;

namespace WinTrains;

public partial class MainForm : Form
{
    private readonly PerSecondTimedStat _fps = InstrumentationBag.Add<PerSecondTimedStat>("SKControl-FPS");
    private readonly ElapsedMillisecondsTimedStat _drawTime = InstrumentationBag.Add<ElapsedMillisecondsTimedStat>("SKControl-DrawTime");

    private readonly IGame _game;
    private readonly IInteractionManager _interactionManager;
    private bool _presenting = true;

    public MainForm()
    {
        InitializeComponent();

        this.Text = "Trains - @davidwengier - " + ThisAssembly.AssemblyInformationalVersion;

        _game = DI.ServiceLocator.GetService<IGame>();
        _interactionManager = DI.ServiceLocator.GetService<IInteractionManager>();

        _game.InitializeAsync(200, 200).GetAwaiter().GetResult();

        _game.SetSize(_skControl.Width, _skControl.Height);

        _skControl.SizeChanged += (s, e) => _game.SetSize(_skControl.Width, _skControl.Height);
        _skControl.MouseDown += SKControl_MouseDown;
        _skControl.MouseMove += SKControl_MouseMove;
        _skControl.MouseUp += SKControl_MouseUp;
        _skControl.MouseWheel += SKControl_MouseWheel;

        _ = PresentLoop();
    }

    private void SKControl_MouseWheel(object? sender, MouseEventArgs e)
    {
        if (e.Delta > 0)
        {
            _interactionManager.PointerZoomIn(e.X, e.Y);
        }
        else
        {
            _interactionManager.PointerZoomOut(e.X, e.Y);
        }
    }

    private void SKControl_MouseDown(object? sender, MouseEventArgs e)
    {
        if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
        {
            _interactionManager.PointerClick(e.X, e.Y);
        }
        else if ((e.Button & MouseButtons.Right) == MouseButtons.Right)
        {
            _interactionManager.PointerAlternateClick(e.X, e.Y);
        }
    }

    private void SKControl_MouseMove(object? sender, MouseEventArgs e)
    {
        if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
        {
            _interactionManager.PointerDrag(e.X, e.Y);
        }
        else if ((e.Button & MouseButtons.Right) == MouseButtons.Right)
        {
            _interactionManager.PointerAlternateDrag(e.X, e.Y);
        }
        else
        {
            _interactionManager.PointerMove(e.X, e.Y);
        }
    }

    private void SKControl_MouseUp(object? sender, MouseEventArgs e)
    {
        if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
        {
            _interactionManager.PointerRelease(e.X, e.Y);
        }
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        _presenting = false;
        DI.ServiceLocator.GetService<IGameStateManager>().Save();
        _game.Dispose();
    }

    private void SKControl_PaintSurface(object sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e)
    {
        using var canvas = new SKCanvasWrapper(e.Surface.Canvas);
        _game.Render(canvas);
    }

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
