using System;
using System.Collections.Generic;

namespace Trains.NET.Rendering
{
    public class PaintBrush : IEquatable<PaintBrush?>
    {
        public Color? Color { get; internal set; }
        public PaintStyle? Style { get; internal set; }
        public int? TextSize { get; internal set; }
        public TextAlign? TextAlign { get; internal set; }
        public int? StrokeWidth { get; internal set; }
        public bool? IsAntialias { get; internal set; }

        public override bool Equals(object? obj)
        {
            return Equals(obj as PaintBrush);
        }

        public bool Equals(PaintBrush? other)
        {
            return other != null &&
                   EqualityComparer<Color?>.Default.Equals(this.Color, other.Color) &&
                   this.Style == other.Style &&
                   this.TextSize == other.TextSize &&
                   this.TextAlign == other.TextAlign &&
                   this.StrokeWidth == other.StrokeWidth &&
                   this.IsAntialias == other.IsAntialias;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Color, this.Style, this.TextSize, this.TextAlign, this.StrokeWidth, this.IsAntialias);
        }
    }
}
