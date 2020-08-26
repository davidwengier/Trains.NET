
namespace Trains.NET.Rendering
{
    public record PaintBrush
    {
        public Color? Color { get; init; }
        public PaintStyle? Style { get; init; }
        public int? TextSize { get; init; }
        public float? StrokeWidth { get; init; }
        public bool? IsAntialias { get; init; }
    }

#pragma warning disable IDE1006 // Naming Styles
    public record Color(string HexCode);

    public record Rectangle(float Left, float Top, float Right, float Bottom);
#pragma warning restore IDE1006 // Naming Styles
}

namespace System.Runtime.CompilerServices
{
    public static class IsExternalInit { }
}
