using SkiaSharp;
using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    internal class TrackRenderer : ITrackRenderer
    {
        private readonly ITrackParameters _parameters;

        public TrackRenderer(ITrackParameters parameters)
        {
            _parameters = parameters;
        }

        public void Render(SKCanvas canvas, Track track, int width)
        {
            if (track.Direction == TrackDirection.Cross)
            {
                RenderStraightTrack(canvas, width);
                canvas.RotateDegrees(90, _parameters.CellSize / 2, _parameters.CellSize / 2);
                RenderStraightTrack(canvas, width);
            }
            else if (track.Direction == TrackDirection.Vertical || track.Direction == TrackDirection.Horizontal)
            {
                if (track.Direction == TrackDirection.Vertical)
                {
                    canvas.RotateDegrees(90, _parameters.CellSize / 2, _parameters.CellSize / 2);
                }
                RenderStraightTrack(canvas, width);
            }
            else
            {
                if (track.Direction == TrackDirection.RightUp || track.Direction == TrackDirection.RightUpDown)
                {
                    canvas.RotateDegrees(90, _parameters.CellSize / 2, _parameters.CellSize / 2);
                }
                else if (track.Direction == TrackDirection.RightDown || track.Direction == TrackDirection.LeftRightDown)
                {
                    canvas.RotateDegrees(180, _parameters.CellSize / 2, _parameters.CellSize / 2);
                }
                else if (track.Direction == TrackDirection.LeftDown || track.Direction == TrackDirection.LeftUpDown)
                {
                    canvas.RotateDegrees(270, _parameters.CellSize / 2, _parameters.CellSize / 2);
                }
                bool drawExtra = track.Direction == TrackDirection.RightUpDown ||
                    track.Direction == TrackDirection.LeftRightDown ||
                    track.Direction == TrackDirection.LeftUpDown ||
                    track.Direction == TrackDirection.LeftRightUp;

                RenderCornerTrack(canvas, width, drawExtra);
            }
        }

        public void RenderStraightTrack(SKCanvas canvas, int width)
        {
            float plankGap = width / _parameters.NumPlanks;
            for (int i = 1; i < _parameters.NumPlanks + 1; i++)
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

            canvas.DrawRect(0, _parameters.TrackPadding, width, _parameters.TrackWidth, clear);
            canvas.DrawRect(0, width - _parameters.TrackPadding - _parameters.TrackWidth, width, _parameters.TrackWidth, clear);

            using var trackPath = new SKPath();
            trackPath.MoveTo(0, _parameters.TrackPadding);
            trackPath.LineTo(width, _parameters.TrackPadding);
            trackPath.MoveTo(0, _parameters.TrackPadding + _parameters.TrackWidth);
            trackPath.LineTo(width, _parameters.TrackPadding + _parameters.TrackWidth);

            trackPath.MoveTo(0, width - _parameters.TrackPadding - _parameters.TrackWidth);
            trackPath.LineTo(width, width - _parameters.TrackPadding - _parameters.TrackWidth);
            trackPath.MoveTo(0, width - _parameters.TrackPadding);
            trackPath.LineTo(width, width - _parameters.TrackPadding);

            using var trackPaint = new SKPaint
            {
                Color = SKColors.Black,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 1,
                IsAntialias = false
            };

            canvas.DrawPath(trackPath, trackPaint);
        }

        private void DrawPlank(SKCanvas canvas, int width, float pos)
        {
            using var path = new SKPath();
            path.MoveTo(pos, _parameters.PlankPadding);
            path.LineTo(pos, width - _parameters.PlankPadding);
            using var plank = new SKPaint
            {
                Color = SKColors.Black,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = _parameters.PlankWidth,
                IsAntialias = true
            };
            canvas.DrawPath(path, plank);
        }

        public void RenderCornerTrack(SKCanvas canvas, int width, bool drawExtra)
        {
            canvas.Save();

            // Rotate to initial angle
            canvas.RotateDegrees(-_parameters.CornerEdgeOffsetDegrees);

            for (int i = 0; i < _parameters.NumCornerPlanks; i++)
            {
                DrawPlank(canvas, width, 0);
                canvas.RotateDegrees(-_parameters.CornerStepDegrees);
            }
            canvas.Restore();

            DrawTracks(canvas, width);

            if (drawExtra)
            {
                canvas.RotateDegrees(90, _parameters.CellSize / 2, _parameters.CellSize / 2);

                DrawTracks(canvas, width);
            }

            void DrawTracks(SKCanvas canvas, int width)
            {
                DrawArc(canvas, _parameters.TrackPadding + (_parameters.TrackWidth / 2), _parameters.TrackWidth, SKColors.White);
                DrawArc(canvas, _parameters.TrackPadding, 1, SKColors.Black);
                DrawArc(canvas, _parameters.TrackPadding + _parameters.TrackWidth, 1, SKColors.Black);

                DrawArc(canvas, width - _parameters.TrackPadding - (_parameters.TrackWidth / 2), _parameters.TrackWidth, SKColors.White);
                DrawArc(canvas, width - _parameters.TrackPadding, 1, SKColors.Black);
                DrawArc(canvas, width - _parameters.TrackPadding - _parameters.TrackWidth, 1, SKColors.Black);
            }

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
