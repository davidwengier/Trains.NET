using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using SkiaSharp.Views.WPF;
using Trains.NET.Engine;
using Trains.NET.Instrumentation;
using Trains.NET.Rendering;
using Trains.NET.Rendering.Skia;
using Trains.Storage;

namespace Trains
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string _windowSizeFileName = FileSystemStorage.GetFilePath("WindowSize.txt");

        private readonly PerSecondTimedStat _fps = InstrumentationBag.Add<PerSecondTimedStat>("Real-FPS");
        private readonly ElapsedMillisecondsTimedStat _drawTime = InstrumentationBag.Add<ElapsedMillisecondsTimedStat>("Real Draw Time");
        private readonly IGame _game;
        private readonly IGameStorage _gameStorage;
        private readonly ITerrainMap _terrainMap;
        private readonly ILayout _trackLayout;
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

            _skElement.MouseDown += _skElement_MouseDown;
            _skElement.MouseMove += _skElement_MouseMove;
            _skElement.MouseUp += _skElement_MouseUp;

            this.Content = _skElement;

            _game = DI.ServiceLocator.GetService<IGame>();
            _gameStorage = DI.ServiceLocator.GetService<IGameStorage>();
            _terrainMap = DI.ServiceLocator.GetService<ITerrainMap>();
            _trackLayout = DI.ServiceLocator.GetService<ILayout>();
            _interactionManager = DI.ServiceLocator.GetService<IInteractionManager>();

            _ = PresentLoop();

            this.Title = "Trains - " + ThisAssembly.AssemblyInformationalVersion;

            SizeChanged += MainWindow_SizeChanged;
        }

        private void _skElement_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton != System.Windows.Input.MouseButtonState.Pressed)
            {
                return;
            }

            var mousePos = e.GetPosition(_skElement);
            _interactionManager.PointerMove((int)mousePos.X, (int)mousePos.Y);
        }

        private void _skElement_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton != System.Windows.Input.MouseButtonState.Pressed)
            {
                return;
            }

            var mousePos = e.GetPosition(_skElement);
            _interactionManager.PointerClick((int)mousePos.X, (int)mousePos.Y);
        }

        private void _skElement_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
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

        public void Save()
        {
            _gameStorage.WriteStaticEntities(_trackLayout);
            _gameStorage.WriteTerrain(_terrainMap);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Save();
            _presenting = false;
            _game.Dispose();
            File.WriteAllText(_windowSizeFileName, $"{this.Width},{this.Height}");
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _game.SetSize((int)e.NewSize.Width, (int)e.NewSize.Height);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show("An error has occurred:\n\n" + e.ExceptionObject.ToString());
        }
    }
}
