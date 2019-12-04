using System;
using SkiaSharp;
using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    internal class TrackRenderer : ITrackRenderer
    {
        private const int PlankWidth = 3;
        private const int NumPlanks = 3;
        private const int NumCornerPlanks = NumPlanks + 1;
        private const int CornerEdgeOffsetDegrees = 10;
        private const int CornerStepDegrees = 
            // Initial angle to draw is 90 degrees, but CornerStepDegrees is only for the middle planks
            // so remove the first and last from the swept angle
            (90 - 2 * CornerEdgeOffsetDegrees) 
            // Now just split up the remainder amongst the middle planks
            / (NumCornerPlanks - 1);
        private const int PlankPadding = 5;
        private const int TrackPadding = 10;
        private const int TrackWidth = 4;

        public void Render(SKCanvas canvas, Track track, int width)
        {
            if (track.Direction == TrackDirection.Vertical)
            {
                canvas.RotateDegrees(90, Game.CellSize / 2, Game.CellSize / 2);
                RenderStraightTrack(canvas, width);
            }
            else if (track.Direction == TrackDirection.Horizontal)
            {
                RenderStraightTrack(canvas, width);
            }
            else if (track.Direction == TrackDirection.LeftUp)
            {
                RenderCornerTrack(canvas, width);
            }
        }

        public void RenderStraightTrack(SKCanvas canvas, int width)
        {
            float plankGap = width / NumPlanks;
            for (int i = 1; i < NumPlanks + 1; i++)
            {
                float pos = (i * plankGap) - (plankGap / 2);
                DrawPlank(canvas, width, pos);
            }

            using var clear = new SKPaint
            {
                Color = SKColors.White,
                Style = SKPaintStyle.Fill,
                StrokeWidth = 0
            };

            canvas.DrawRect(0, TrackPadding, width, TrackWidth, clear);
            canvas.DrawRect(0, width - TrackPadding - TrackWidth, width, TrackWidth, clear);

            using var trackPath = new SKPath();
            trackPath.MoveTo(0, TrackPadding);
            trackPath.LineTo(width, TrackPadding);
            trackPath.MoveTo(0, TrackPadding + TrackWidth);
            trackPath.LineTo(width, TrackPadding + TrackWidth);

            trackPath.MoveTo(0, width - TrackPadding - TrackWidth);
            trackPath.LineTo(width, width - TrackPadding - TrackWidth);
            trackPath.MoveTo(0, width - TrackPadding);
            trackPath.LineTo(width, width - TrackPadding);

            using var trackPaint = new SKPaint
            {
                Color = SKColors.Black,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 1,
                IsAntialias = false
            };

            canvas.DrawPath(trackPath, trackPaint);
        }

        private static void DrawPlank(SKCanvas canvas, int width, float pos)
        {
            using var path = new SKPath();
            path.MoveTo(pos, PlankPadding);
            path.LineTo(pos, width - PlankPadding);
            using var plank = new SKPaint
            {
                Color = SKColors.Black,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = PlankWidth,
                IsAntialias = true
            };
            canvas.DrawPath(path, plank);
        }

        public void RenderCornerTrack(SKCanvas canvas, int width)
        {
            canvas.Save();

            // Rotate to initial angle
            canvas.RotateDegrees(-CornerEdgeOffsetDegrees);

            for (int i = 0; i < NumCornerPlanks; i++)
            {
                DrawPlank(canvas, width, 0);
                canvas.RotateDegrees(-CornerStepDegrees);
            }
            canvas.Restore();

            DrawArc(canvas, TrackPadding + (TrackWidth / 2), TrackWidth, SKColors.White);
            DrawArc(canvas, TrackPadding, 1, SKColors.Black);
            DrawArc(canvas, TrackPadding + TrackWidth, 1, SKColors.Black);

            DrawArc(canvas, width - TrackPadding - (TrackWidth / 2), TrackWidth, SKColors.White);
            DrawArc(canvas, width - TrackPadding, 1, SKColors.Black);
            DrawArc(canvas, width - TrackPadding - TrackWidth, 1, SKColors.Black);

            static void DrawArc(SKCanvas canvas, float position, int strokeWidth, SKColor color)
            {
                // Offset to match other tracks 
                position += 0.5f;
                using var trackPath = new SKPath();
                trackPath.MoveTo(0, position);
                trackPath.ArcTo(position, position, 0, SKPathArcSize.Small, SKPathDirection.CounterClockwise, position, 0);

                using var trackPaint = new SKPaint()
                {
                    Style = SKPaintStyle.Stroke,
                    StrokeWidth = strokeWidth,
                    Color = color,
                    IsAntialias = true
                };
                canvas.DrawPath(trackPath, trackPaint);
            }
        }
    }
}
