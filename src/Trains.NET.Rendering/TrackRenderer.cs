using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    internal class TrackRenderer : ITrackRenderer //, IDisposable
    {
        private readonly ITrackParameters _parameters;
        private readonly IPathFactory _pathFactory;

        private readonly PaintBrush _straightTrackClear;
        private readonly PaintBrush _straightTrackPaint;
        private readonly PaintBrush _arcTrackPaint;
        private readonly PaintBrush _arcTrackClear;
        private readonly PaintBrush _plankPaint;
        
        public TrackRenderer(ITrackParameters parameters, IPathFactory pathFactory)
        {
            _parameters = parameters;
            _pathFactory = pathFactory;
            _plankPaint = new PaintBrush
            {
                Color = Colors.Black,
                Style = PaintStyle.Stroke,
                IsAntialias = true
            };
            _arcTrackClear = new PaintBrush()
            {
                Style = PaintStyle.Stroke,
                Color = Colors.White,
                IsAntialias = true
            };
            _arcTrackPaint = new PaintBrush()
            {
                Style = PaintStyle.Stroke,
                StrokeWidth = 1,
                Color = Colors.Black,
                IsAntialias = true
            };
            _straightTrackPaint = new PaintBrush
            {
                Color = Colors.Black,
                Style = PaintStyle.Stroke,
                StrokeWidth = 1,
                IsAntialias = false
            };
            _straightTrackClear = new PaintBrush
            {
                Color = Colors.White,
                Style = PaintStyle.Fill,
                StrokeWidth = 0
            };
        }

        public void Render(ICanvas canvas, Track track, int width)
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

        public void RenderStraightTrack(ICanvas canvas, int width)
        {
            float plankGap = width / _parameters.NumPlanks;
            for (int i = 1; i < _parameters.NumPlanks + 1; i++)
            {
                float pos = (i * plankGap) - (plankGap / 2);
                DrawPlank(canvas, width, pos);
            }

            canvas.DrawRect(0, _parameters.TrackPadding, width, _parameters.TrackWidth, _straightTrackClear);
            canvas.DrawRect(0, width - _parameters.TrackPadding - _parameters.TrackWidth, width, _parameters.TrackWidth, _straightTrackClear);

            var trackPath = _pathFactory.Create();
            trackPath.MoveTo(0, _parameters.TrackPadding);
            trackPath.LineTo(width, _parameters.TrackPadding);
            trackPath.MoveTo(0, _parameters.TrackPadding + _parameters.TrackWidth);
            trackPath.LineTo(width, _parameters.TrackPadding + _parameters.TrackWidth);

            trackPath.MoveTo(0, width - _parameters.TrackPadding - _parameters.TrackWidth);
            trackPath.LineTo(width, width - _parameters.TrackPadding - _parameters.TrackWidth);
            trackPath.MoveTo(0, width - _parameters.TrackPadding);
            trackPath.LineTo(width, width - _parameters.TrackPadding);

            canvas.DrawPath(trackPath, _straightTrackPaint);
        }

        private void DrawPlank(ICanvas canvas, int width, float pos)
        {
            _plankPaint.StrokeWidth = _parameters.PlankWidth;

            var path = _pathFactory.Create();
            path.MoveTo(pos, _parameters.PlankPadding);
            path.LineTo(pos, width - _parameters.PlankPadding);
            canvas.DrawPath(path, _plankPaint);
        }

        public void RenderCornerTrack(ICanvas canvas, int width, bool drawExtra)
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

            void DrawTracks(ICanvas canvas, int width)
            {
                _arcTrackClear.StrokeWidth = _parameters.TrackWidth;

                DrawArc(canvas, _parameters.TrackPadding + (_parameters.TrackWidth / 2), _arcTrackClear);
                DrawArc(canvas, _parameters.TrackPadding, _arcTrackPaint);
                DrawArc(canvas, _parameters.TrackPadding + _parameters.TrackWidth, _arcTrackPaint);

                DrawArc(canvas, width - _parameters.TrackPadding - (_parameters.TrackWidth / 2), _arcTrackClear);
                DrawArc(canvas, width - _parameters.TrackPadding, _arcTrackPaint);
                DrawArc(canvas, width - _parameters.TrackPadding - _parameters.TrackWidth, _arcTrackPaint);
            }

            void DrawArc(ICanvas canvas, float position, PaintBrush trackPaint)
            {
                // Offset to match other tracks 
                var trackPath = _pathFactory.Create();
                trackPath.MoveTo(0, position);
                trackPath.ArcTo(position, position, 0, PathArcSize.Small, PathDirection.CounterClockwise, position, 0);

                canvas.DrawPath(trackPath, trackPaint);
            }
        }

        //public void Dispose()
        //{
        //    _straightTrackClear.Dispose();
        //    _straightTrackPaint.Dispose();
        //    _arcTrackPaint.Dispose();
        //    _arcTrackClear.Dispose();
        //    _plankPaint.Dispose();
        //}
    }
}
