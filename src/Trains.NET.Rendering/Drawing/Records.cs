using System;

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
#pragma warning disable format
    public record Color(int A, int R, int G, int B)
    {
        public Color(string hexCode)
            : this(HtmlToArgb(hexCode))
        {
        }

        private static Color HtmlToArgb(string htmlColor)
        {
            if (htmlColor.Length == 7)
            {
                return new Color(byte.MaxValue,
                    Convert.ToByte(htmlColor.Substring(1, 2), 16),
                    Convert.ToByte(htmlColor.Substring(3, 2), 16),
                    Convert.ToByte(htmlColor.Substring(5, 2), 16));
            }
            else if (htmlColor.Length == 9)
            {
                return new Color(
                    Convert.ToByte(htmlColor.Substring(1, 2), 16),
                    Convert.ToByte(htmlColor.Substring(3, 2), 16),
                    Convert.ToByte(htmlColor.Substring(5, 2), 16),
                    Convert.ToByte(htmlColor.Substring(7, 2), 16));
            }
            throw new ArgumentException($"Invalid color code '{htmlColor}', must be #RRGGBB or #AARRGGBB", nameof(htmlColor));
        }
    }

    public record Rectangle(float Left, float Top, float Right, float Bottom);
#pragma warning restore IDE1006 // Naming Styles
#pragma warning restore format
}

namespace System.Runtime.CompilerServices
{
    public static class IsExternalInit { }
}
