using SkiaSharp;
using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    internal class TrackRenderer
    {
        public void Render(SKCanvas canvas, Track track, int width)
        {
            const int plankWidth = 3;
            const int numPlanks = 3;
            const int plankPadding = 5;
            const int trackPadding = 10;
            const int trackWidth = 4;

            float plankGap = width / numPlanks;
            using var path = new SKPath();
            for (int i = 1; i < numPlanks + 1; i++)
            {
                float pos = (i * plankGap) - (plankGap / 2);

                path.MoveTo(pos, plankPadding);
                path.LineTo(pos, width - plankPadding);
            }
            using var plank = new SKPaint
            {
                Color = SKColors.Black,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = plankWidth
            };
            canvas.DrawPath(path, plank);

            using var clear = new SKPaint
            {
                Color = SKColors.White,
                Style = SKPaintStyle.Fill,
                StrokeWidth = 0
            };

            canvas.DrawRect(0, trackPadding, width, trackWidth, clear);
            canvas.DrawRect(0, width - trackPadding - trackWidth, width, trackWidth, clear);

            using var trackPath = new SKPath();
            trackPath.MoveTo(0, trackPadding);
            trackPath.LineTo(width, trackPadding);
            trackPath.MoveTo(0, trackPadding + trackWidth);
            trackPath.LineTo(width, trackPadding + trackWidth);

            trackPath.MoveTo(0, width - trackPadding - trackWidth);
            trackPath.LineTo(width, width - trackPadding - trackWidth);
            trackPath.MoveTo(0, width - trackPadding);
            trackPath.LineTo(width, width - trackPadding);

            using var trackPaint = new SKPaint
            {
                Color = SKColors.Black,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 1
            };

            canvas.DrawPath(trackPath, trackPaint);
        }
    }
}
