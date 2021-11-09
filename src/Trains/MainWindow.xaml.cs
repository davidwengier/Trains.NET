using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using Trains.NET.Engine;
using Trains.NET.Rendering;
using Trains.Storage;

namespace Trains;

public partial class MainWindow : Window
{
    private readonly string _windowSizeFileName = FileSystemStorage.GetFilePath("WindowSize.txt");
    private readonly IGame _game;
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
        _interactionManager = DI.ServiceLocator.GetService<IInteractionManager>();

        _gameElement = new GameElement(_game);

        _gameElement.MouseDown += SKElement_MouseDown;
        _gameElement.MouseMove += SKElement_MouseMove;
        _gameElement.MouseUp += SKElement_MouseUp;
        _gameElement.MouseWheel += SKElement_MouseWheel;

        this.Content = _gameElement;

        this.Title = "Trains - @davidwengier - " + ThisAssembly.AssemblyInformationalVersion;

        _gameElement.SizeChanged += SKElement_SizeChanged;

        _game.InitializeAsync(200, 200).GetAwaiter().GetResult();
    }

    private void SKElement_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
    {
        var mousePos = e.GetPosition(_gameElement);

        if (e.Delta > 0)
        {
            _interactionManager.PointerZoomIn((int)mousePos.X, (int)mousePos.Y);
        }
        else
        {
            _interactionManager.PointerZoomOut((int)mousePos.X, (int)mousePos.Y);
        }
    }

    private void SKElement_MouseMove(object? sender, System.Windows.Input.MouseEventArgs e)
    {
        var mousePos = e.GetPosition(_gameElement);

        if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
        {
            _interactionManager.PointerDrag((int)mousePos.X, (int)mousePos.Y);
        }
        else if (e.RightButton == System.Windows.Input.MouseButtonState.Pressed)
        {
            _interactionManager.PointerAlternateDrag((int)mousePos.X, (int)mousePos.Y);
        }
        else
        {
            _interactionManager.PointerMove((int)mousePos.X, (int)mousePos.Y);
        }
    }

    private void SKElement_MouseDown(object? sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        var mousePos = e.GetPosition(_gameElement);

        if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
        {
            _interactionManager.PointerClick((int)mousePos.X, (int)mousePos.Y);
        }
        else if (e.RightButton == System.Windows.Input.MouseButtonState.Pressed)
        {
            _interactionManager.PointerAlternateClick((int)mousePos.X, (int)mousePos.Y);
        }
    }

    private void SKElement_MouseUp(object? sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (e.ChangedButton != System.Windows.Input.MouseButton.Left)
        {
            return;
        }

        var mousePos = e.GetPosition(_gameElement);
        _interactionManager.PointerRelease((int)mousePos.X, (int)mousePos.Y);
    }

    protected override void OnClosing(CancelEventArgs e)
    {
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
