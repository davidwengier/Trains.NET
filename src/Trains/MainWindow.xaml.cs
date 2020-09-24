using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using Trains.NET.Engine;
using Trains.NET.Rendering;
using Trains.Storage;

namespace Trains
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string _windowSizeFileName = FileSystemStorage.GetFilePath("WindowSize.txt");
        private readonly IGame _game;
        private readonly IGameStorage _gameStorage;
        private readonly ITerrainMap _terrainMap;
        private readonly ILayout _trackLayout;
        private readonly GameElement _gameElement;
        private readonly IInteractionManager _interactionManager;

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

            _game = DI.ServiceLocator.GetService<IGame>();
            _gameStorage = DI.ServiceLocator.GetService<IGameStorage>();
            _terrainMap = DI.ServiceLocator.GetService<ITerrainMap>();
            _trackLayout = DI.ServiceLocator.GetService<ILayout>();
            _interactionManager = DI.ServiceLocator.GetService<IInteractionManager>();

            _gameElement = new GameElement(_game);

            _gameElement.MouseDown += _skElement_MouseDown;
            _gameElement.MouseMove += _skElement_MouseMove;
            _gameElement.MouseUp += _skElement_MouseUp;

            this.Content = _gameElement;

            this.Title = "Trains - " + ThisAssembly.AssemblyInformationalVersion;

            _gameElement.SizeChanged += MainWindow_SizeChanged;
        }

        private void _skElement_MouseMove(object? sender, System.Windows.Input.MouseEventArgs e)
        {
            var mousePos = e.GetPosition(_gameElement);
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                _interactionManager.PointerDrag((int)mousePos.X, (int)mousePos.Y);
            }
            else
            {
                _interactionManager.PointerMove((int)mousePos.X, (int)mousePos.Y);
            }
        }

        private void _skElement_MouseDown(object? sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton != System.Windows.Input.MouseButtonState.Pressed)
            {
                return;
            }

            var mousePos = e.GetPosition(_gameElement);
            _interactionManager.PointerClick((int)mousePos.X, (int)mousePos.Y);
        }

        private void _skElement_MouseUp(object? sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton != System.Windows.Input.MouseButton.Left)
            {
                return;
            }

            var mousePos = e.GetPosition(_gameElement);
            _interactionManager.PointerRelease((int)mousePos.X, (int)mousePos.Y);
        }

        public void Save()
        {
            _gameStorage.WriteStaticEntities(_trackLayout);
            _gameStorage.WriteTerrain(_terrainMap);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Save();
            _game.Dispose();
            File.WriteAllText(_windowSizeFileName, $"{this.Width},{this.Height}");
        }

        private void MainWindow_SizeChanged(object? sender, SizeChangedEventArgs e)
        {
            _game.SetSize((int)e.NewSize.Width, (int)e.NewSize.Height);
        }

        private void CurrentDomain_UnhandledException(object? sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show("An error has occurred:\n\n" + e.ExceptionObject.ToString());
        }
    }
}
