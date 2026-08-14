using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Trains.NET.Rendering;
using Trains.Storage;

namespace Trains;

public partial class MainWindow : Window
{
    private readonly string _windowSizeFileName = FileSystemStorage.GetFilePath("WindowSize.txt");
    private readonly IGame _game;
    private readonly GameElement _gameElement;
    private readonly IInteractionManager _interactionManager;
    private PendingPointerMove? _pendingPointerMove;

    public MainWindow()
    {
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

        InitializeComponent();

        if (File.Exists(_windowSizeFileName))
        {
            var sizeString = File.ReadAllText(_windowSizeFileName);
            var bits = sizeString.Split(',');
            if (bits.Length == 2)
            {
                if (double.TryParse(bits[0], out var width) && double.TryParse(bits[1], out var height))
                {
                    Width = width;
                    Height = height;
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

        Content = _gameElement;

        Title = "Trains - @davidwengier@aus.social - " + ThisAssembly.AssemblyInformationalVersion;

        _game.InitializeAsync(200, 200).GetAwaiter().GetResult();
    }

    private void SKElement_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
    {
        (var x, var y) = _gameElement.ToPixels(e.GetPosition(_gameElement));

        if (e.Delta > 0)
        {
            _interactionManager.PointerZoomIn(x, y);
        }
        else
        {
            _interactionManager.PointerZoomOut(x, y);
        }
    }

    private void SKElement_MouseMove(object? sender, System.Windows.Input.MouseEventArgs e)
    {
        (var x, var y) = _gameElement.ToPixels(e.GetPosition(_gameElement));
        var pointerMoveQueued = _pendingPointerMove is not null;
        _pendingPointerMove = new PendingPointerMove(x, y, e.LeftButton, e.RightButton);

        if (pointerMoveQueued)
        {
            return;
        }

        // Sustained WPF mouse input can starve GLWpfControl rendering. Coalesce moves
        // at render priority so the latest pointer position is rendered between batches.
        Dispatcher.BeginInvoke(DispatcherPriority.Render, ProcessPointerMove);
    }

    private void ProcessPointerMove()
    {
        if (_pendingPointerMove is not { } pointerMove)
        {
            return;
        }

        _pendingPointerMove = null;

        if (pointerMove.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
        {
            _interactionManager.PointerDrag(pointerMove.X, pointerMove.Y);
        }
        else if (pointerMove.RightButton == System.Windows.Input.MouseButtonState.Pressed)
        {
            _interactionManager.PointerAlternateDrag(pointerMove.X, pointerMove.Y);
        }
        else
        {
            _interactionManager.PointerMove(pointerMove.X, pointerMove.Y);
        }

        // Complete the invalidated GL render now rather than letting more input defer it.
        _gameElement.InvalidateVisual();
        _gameElement.UpdateLayout();
    }

    private void SKElement_MouseDown(object? sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        _pendingPointerMove = null;
        (var x, var y) = _gameElement.ToPixels(e.GetPosition(_gameElement));

        if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
        {
            _interactionManager.PointerClick(x, y);
        }
        else if (e.RightButton == System.Windows.Input.MouseButtonState.Pressed)
        {
            _interactionManager.PointerAlternateClick(x, y);
        }
    }

    private void SKElement_MouseUp(object? sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        _pendingPointerMove = null;

        if (e.ChangedButton != System.Windows.Input.MouseButton.Left)
        {
            return;
        }

        (var x, var y) = _gameElement.ToPixels(e.GetPosition(_gameElement));
        _interactionManager.PointerRelease(x, y);
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        _game.Dispose();
        File.WriteAllText(_windowSizeFileName, $"{Width},{Height}");
    }

    private void CurrentDomain_UnhandledException(object? sender, UnhandledExceptionEventArgs e)
    {
        MessageBox.Show("An error has occurred:\n\n" + e.ExceptionObject.ToString());
    }

    private readonly record struct PendingPointerMove(int X, int Y, MouseButtonState LeftButton, MouseButtonState RightButton);
}
