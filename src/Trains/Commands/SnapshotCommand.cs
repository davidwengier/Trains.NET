using System.Windows;
using SkiaSharp;
using SkiaSharp.Views.WPF;
using Trains.NET.Engine;
using Trains.NET.Rendering;
using Trains.NET.Rendering.Skia;

namespace Trains.Commands;

[Order(2000)]
public class SnapshotCommand : ICommand
{
    private readonly IPixelMapper _pixelMapper;
    private readonly IEnumerable<ILayerRenderer> _renderers;

    public string Name => "Snapshot";

    public SnapshotCommand(IPixelMapper pixelMapper, IEnumerable<ILayerRenderer> renderers)
    {
        _pixelMapper = pixelMapper;
        _renderers = renderers;
    }

    public void Execute()
    {
        var pixelMapper = _pixelMapper.Snapshot();
        (int width, int height) = (pixelMapper.ViewPortWidth, pixelMapper.ViewPortHeight);
        using var bitmap = new SKBitmap(width, height, SKImageInfo.PlatformColorType, SKAlphaType.Premul);
        using var skCanvas = new SKCanvas(bitmap);
        using (ICanvas canvas = new SKCanvasWrapper(skCanvas))
        {
            foreach (var renderer in _renderers)
            {
                if (!renderer.Enabled) continue;
                using var _ = canvas.Scope();
                renderer.Render(canvas, width, height, pixelMapper);
            }
        }
        Clipboard.SetImage(bitmap.ToWriteableBitmap());
    }
}
