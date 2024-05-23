using System.Reflection;
using System.Runtime.InteropServices;
using Silk.NET.Core;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using SilkTrains;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SkiaSharp;
using Trains.NET.Rendering;
using Trains.NET.Rendering.Skia;
using Trains.Storage;

// Game stuff
string windowSizeFileName = FileSystemStorage.GetFilePath("WindowSize.txt");
IGame game = DI.ServiceLocator.GetService<IGame>();
IInteractionManager interactionManager = DI.ServiceLocator.GetService<IInteractionManager>();

// Get size from file
Vector2D<int> size = new(1280, 720);
if (File.Exists(windowSizeFileName))
{
    string sizeString = File.ReadAllText(windowSizeFileName);
    string[] bits = sizeString.Split(',');
    if (bits.Length == 2)
    {
        if (double.TryParse(bits[0], out double width) && double.TryParse(bits[1], out double height))
        {
            size = new((int)width, (int)height);
        }
    }
}

// Create the Silk.NET window
var options = WindowOptions.Default with
{
    Size = size,
    Title = "Trains - @davidwengier@aus.socual - " + ThisAssembly.AssemblyInformationalVersion,
    PreferredStencilBufferBits = 8,
    PreferredBitDepth = new(8)
};

var window = Window.Create(options);

// Silk.NET-Skia interop variables
GRGlInterface grGlInterface = null!;
GRContext grContext = null!;
GRBackendRenderTarget renderTarget = null!;
SKSurface surface = null!;
SKCanvas canvas = null!;
SKCanvasWrapper canvasWrapper = null!;

// Other Silk.NET variables
IInputContext inputContext = null!;

// Input handling
void BindMouse(IMouse mouse)
{
    mouse.Scroll += (_, deltaPos) =>
    {
        var mousePos = window.NotBuggedPointToFramebuffer((Vector2D<int>)mouse.Position.ToGeneric());
        if (deltaPos.Y > 0)
        {
            interactionManager.PointerZoomIn(mousePos.X, mousePos.Y);
        }
        else
        {
            interactionManager.PointerZoomOut(mousePos.X, mousePos.Y);
        }
    };
    mouse.MouseMove += (_, newPosFloat) =>
    {
        var mousePos = window.NotBuggedPointToFramebuffer((Vector2D<int>)newPosFloat.ToGeneric());
        if (mouse.IsButtonPressed(MouseButton.Left))
        {
            interactionManager.PointerDrag(mousePos.X, mousePos.Y);
        }
        else if (mouse.IsButtonPressed(MouseButton.Right))
        {
            interactionManager.PointerAlternateDrag(mousePos.X, mousePos.Y);
        }
        else
        {
            interactionManager.PointerMove(mousePos.X, mousePos.Y);
        }
    };
    mouse.MouseDown += (_, button) =>
    {
        var mousePos = window.NotBuggedPointToFramebuffer((Vector2D<int>)mouse.Position.ToGeneric());
        if (button == MouseButton.Left)
        {
            interactionManager.PointerClick(mousePos.X, mousePos.Y);
        }
        else if (button == MouseButton.Right)
        {
            interactionManager.PointerAlternateClick(mousePos.X, mousePos.Y);
        }
    };
    mouse.MouseUp += (_, button) =>
    {
        if (button != MouseButton.Left)
        {
            return;
        }

        var mousePos = window.NotBuggedPointToFramebuffer((Vector2D<int>)mouse.Position.ToGeneric());
        interactionManager.PointerRelease(mousePos.X, mousePos.Y);
    };
}

// Create the Skia-OpenGL link
void HandleSize(Vector2D<int> fbSize)
{
    renderTarget = new(window.FramebufferSize.X, window.FramebufferSize.Y, 0, 8, new(0, (int)InternalFormat.Rgba8));
    surface = SKSurface.Create(grContext, renderTarget, GRSurfaceOrigin.BottomLeft, SKColorType.Rgba8888);
    canvas = surface.Canvas;
    canvasWrapper = new(canvas);
    game.SetSize(fbSize.X, fbSize.Y);
}

// Bind window events
window.Load += () =>
{
    grGlInterface = GRGlInterface.Create(name => window.GLContext!.TryGetProcAddress(name, out var addr) ? addr : 0);
    grGlInterface.Validate();
    grContext = GRContext.CreateGl(grGlInterface);
    game.InitializeAsync(200, 200).GetAwaiter().GetResult();
    HandleSize(window.FramebufferSize);
    inputContext = window.CreateInput();
    if (inputContext.Mice.Count > 0)
    {
        BindMouse(inputContext.Mice[0]);
    }

    using var ers = Assembly.GetExecutingAssembly().GetManifestResourceStream("SilkTrains.RedTrain.png");
    if (ers is null)
    {
        return;
    }

    using var img = Image.Load<Rgba32>(ers);
    var rowByteLen = img.Width * 4;
    var bytes = new byte[rowByteLen * img.Height];
    img.ProcessPixelRows(processor =>
    {
        for (int i = 0; i < img.Height; i++)
        {
            MemoryMarshal.Cast<Rgba32, byte>(processor.GetRowSpan(i)).CopyTo(bytes.AsSpan(i * rowByteLen, rowByteLen));
        }
    });

    var rawImage = new RawImage(img.Width, img.Height, bytes);
    window.SetWindowIcon(ref rawImage);
};

window.FramebufferResize += HandleSize;
window.Render += deltaSeconds =>
{
    grContext.ResetContext();
    game.Render(canvasWrapper);
    canvas.Flush();
};

window.Closing += () =>
{
    game.Dispose();
    File.WriteAllText(windowSizeFileName, $"{window.Size.X},{window.Size.Y}");
};

// Open the window!
window.Run();
