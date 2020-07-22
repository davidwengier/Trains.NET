using System;
using System.Collections.Generic;

namespace Trains.NET.Rendering
{
    public class PaintBrush : IEquatable<PaintBrush?>
    {
        public Color? Color { get; set; }
        public PaintStyle? Style { get; set; }
        public int? TextSize { get; set; }
        public TextAlign? TextAlign { get; set; }
        public float? StrokeWidth { get; set; }
        public bool? IsAntialias { get; set; }

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
