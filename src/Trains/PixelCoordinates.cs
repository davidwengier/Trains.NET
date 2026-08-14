using System.Windows;
using System.Windows.Media;

namespace Trains;

internal static class PixelCoordinates
{
    public static (int X, int Y) FromWpf(Visual visual, Point point)
    {
        var source = PresentationSource.FromVisual(visual);
        if (source is null)
        {
            return ((int)point.X, (int)point.Y);
        }

        var pixelPoint = source.CompositionTarget.TransformToDevice.Transform(point);
        return ((int)pixelPoint.X, (int)pixelPoint.Y);
    }
}
