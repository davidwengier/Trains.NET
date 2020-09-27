using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using SkiaSharp.Views.WPF;
using Trains.NET.Instrumentation;
using Trains.NET.Rendering;
using Trains.NET.Rendering.Skia;
using Trains.Storage;

namespace Trains
{
    public partial class MainWindow : Window
    {
        private readonly string _windowSizeFileName = FileSystemStorage.GetFilePath("WindowSize.txt");

        private readonly PerSecondTimedStat _fps = InstrumentationBag.Add<PerSecondTimedStat>("Real-FPS");
        private readonly ElapsedMillisecondsTimedStat _drawTime = InstrumentationBag.Add<ElapsedMillisecondsTimedStat>("Real Draw Time");
        private readonly IGame _game;
        private readonly SKElement _skElement;
        private readonly IInteractionManager _interactionManager;
        private bool _presenting = true;

        public MainWindow()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            InitializeComponent();

            if (File.Exists(_windowSizeFileName))
            {
                string sizeString = File.ReadAllText(_windowSizeFileName);
                string[] bits = sizeString.Split(',');
                if (bits.Length == 2)
                {
                    if (double.TryParse(bits[0], out double width) && double.TryParse(bits[1], out double height))
                    {
                        this.Width = width;
                        this.Height = height;
                    }
                }
            }

            _skElement = new SKElement();
            _skElement.PaintSurface += SKElement_PaintSurface;
            _skElement.MouseDown += SKElement_MouseDown;
            _skElement.MouseMove += SKElement_MouseMove;
            _skElement.MouseUp += SKElement_MouseUp;
            _skElement.SizeChanged += SKElement_SizeChanged;

            this.Content = _skElement;

            _game = DI.ServiceLocator.GetService<IGame>();
            _interactionManager = DI.ServiceLocator.GetService<IInteractionManager>();

            this.Title = "Trains - " + ThisAssembly.AssemblyInformationalVersion;

            _ = PresentLoop();
        }

        private void SKElement_MouseMove(object? sender, System.Windows.Input.MouseEventArgs e)
        {
            var mousePos = e.GetPosition(_skElement);
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                _interactionManager.PointerDrag((int)mousePos.X, (int)mousePos.Y);
            }
            else
            {
                _interactionManager.PointerMove((int)mousePos.X, (int)mousePos.Y);
            }
        }

        private void SKElement_MouseDown(object? sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton != System.Windows.Input.MouseButtonState.Pressed)
            {
                return;
            }

            var mousePos = e.GetPosition(_skElement);
            _interactionManager.PointerClick((int)mousePos.X, (int)mousePos.Y);
        }

        private void SKElement_MouseUp(object? sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton != System.Windows.Input.MouseButton.Left)
            {
                return;
            }

            var mousePos = e.GetPosition(_skElement);
            _interactionManager.PointerRelease((int)mousePos.X, (int)mousePos.Y);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
        private void SKElement_PaintSurface(object? sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e)
        {
            DpiScale dpi = VisualTreeHelper.GetDpi(_skElement);
            e.Surface.Canvas.Scale((float)dpi.DpiScaleX, (float)dpi.DpiScaleY);
            _game.Render(new SKCanvasWrapper(e.Surface.Canvas));
        }

        private async Task PresentLoop()
        {
            while (_presenting)
            {
                _drawTime.Start();

                _skElement.InvalidateVisual();

                _drawTime.Stop();

                _fps.Update();

                await Task.Delay(16).ConfigureAwait(true);
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _presenting = false;
            _game.Dispose();
            File.WriteAllText(_windowSizeFileName, $"{this.Width},{this.Height}");
        }

        private void SKElement_SizeChanged(object? sender, SizeChangedEventArgs e)
        {
            _game.SetSize((int)e.NewSize.Width, (int)e.NewSize.Height);
        }

        private void CurrentDomain_UnhandledException(object? sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show("An error has occurred:\n\n" + e.ExceptionObject.ToString());
        }
    }
}
