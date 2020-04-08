namespace Trains.NET.Rendering
{
    public class PaintBrush
    {
        public Colors? Color { get; internal set; }
        public PaintStyle? Style { get; internal set; }
        public int? TextSize { get; internal set; }
        public TextAlign? TextAlign { get; internal set; }
        public int? StrokeWidth { get; internal set; }
        public bool? IsAntialias { get; internal set; }
    }
}
