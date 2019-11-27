using System;
using SkiaSharp;
using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    internal class TrackRenderer : ITrackRenderer
    {
        private const int PlankWidth = 3;
        private const int NumPlanks = 3;
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
                IsAntialias = true
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
            for (int i = 0; i < NumPlanks; i++)
            {
                canvas.RotateDegrees(-22.5f);
                DrawPlank(canvas, width, 0);
            }
            canvas.Restore();


            float offset = -TrackPadding + 3;
            DrawTrackBackground(canvas, width, offset);

            offset = -TrackPadding - 2;
            DrawTrackBackground(canvas, width * 2, offset);

            offset = -TrackPadding + 2;
            DrawCurvedTracks(canvas, width, offset);

            offset = -TrackPadding - 2;
            DrawCurvedTracks(canvas, width * 2, offset);

            static void DrawTrackBackground(SKCanvas canvas, int width, float offset)
            {
                canvas.Save();
                canvas.Translate(-width / 2, -width / 2);

                using var clear = new SKPaint
                {
                    Color = SKColors.White,
                    Style = SKPaintStyle.Stroke,
                    StrokeWidth = 5,
                    IsAntialias = true
                };
                using var trackPath = new SKPath();
                trackPath.ArcTo(new SKRect(offset, offset, width + offset, width + offset), 0, 90, true);

                canvas.DrawPath(trackPath, clear);
                canvas.Restore();
            }

            static void DrawCurvedTracks(SKCanvas canvas, int width, float offset)
            {
                canvas.Save();
                canvas.Translate(-width / 2, -width / 2);

                using var trackPath = new SKPath();
                trackPath.ArcTo(new SKRect(offset, offset, width + offset, width + offset), 0, 90, true);
                trackPath.ArcTo(new SKRect(offset - TrackWidth, offset - TrackWidth, width + offset + TrackWidth, width + offset + TrackWidth), 0, 90, true);
                using var trackPaint = new SKPaint
                {
                    Color = SKColors.Black,
                    Style = SKPaintStyle.Stroke,
                    StrokeWidth = 1,
                    IsAntialias = true
                };
                canvas.DrawPath(trackPath, trackPaint);
                canvas.Restore();
            }
        }
    }
}
