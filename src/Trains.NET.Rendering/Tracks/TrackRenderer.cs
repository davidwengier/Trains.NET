using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    public class TrackRenderer : ICachableRenderer<Track>
    {
        private readonly ITrackParameters _trackParameters;

        private readonly PaintBrush _trackEdge;
        private readonly PaintBrush _trackClear;
        private readonly PaintBrush _plankPaint;

        private readonly IPath _cornerTrackPath;
        private readonly IPath _cornerPlankPath;
        private readonly IPath _cornerSinglePlankPath;
        private readonly IPath _horizontalTrackPath;
        private readonly IPath _horizontalPlankPath;

        public TrackRenderer(ITrackParameters trackParameters, ITrackPathBuilder trackPathBuilder)
        {
            _trackParameters = trackParameters;

            _cornerTrackPath = trackPathBuilder.BuildCornerTrackPath();
            _cornerPlankPath = trackPathBuilder.BuildCornerPlankPath();
            _cornerSinglePlankPath = trackPathBuilder.BuildCornerPlankPath(1);
            _horizontalTrackPath = trackPathBuilder.BuildHorizontalTrackPath();
            _horizontalPlankPath = trackPathBuilder.BuildHorizontalPlankPath();

            _plankPaint = new PaintBrush
            {
                Color = Colors.Black,
                Style = PaintStyle.Stroke,
                StrokeWidth = _trackParameters.PlankWidth,
                IsAntialias = true
            };
            _trackClear = new PaintBrush
            {
                Color = Colors.White,
                Style = PaintStyle.Stroke,
                StrokeWidth = _trackParameters.RailTopWidth,
                IsAntialias = true
            };
            _trackEdge = new PaintBrush
            {
                Color = Colors.Black,
                Style = PaintStyle.Stroke,
                StrokeWidth = _trackParameters.RailWidth,
                IsAntialias = true
            };
        }

        public string GetCacheKey(Track item) => item.Identifier;

        public void Render(ICanvas canvas, Track track)
        {
            switch (track.Direction)
            {
                case TrackDirection.Horizontal:
                    DrawHorizontal(canvas);
                    break;
                case TrackDirection.Vertical:
                    DrawVertical(canvas);
                    break;
                case TrackDirection.Cross:
                    DrawCross(canvas);
                    break;
                case TrackDirection.LeftUp:
                case TrackDirection.RightUp:
                case TrackDirection.RightDown:
                case TrackDirection.LeftDown:
                case TrackDirection.RightUp_RightDown:
                case TrackDirection.RightDown_LeftDown:
                case TrackDirection.LeftDown_LeftUp:
                case TrackDirection.LeftUp_RightUp:
                    DrawCorner(canvas, track);
                    break;
                case TrackDirection.Undefined:
                default:
                    break;
            }
        }

        private void DrawHorizontal(ICanvas canvas)
        {
            canvas.DrawPath(_horizontalPlankPath, _plankPaint);
            DrawHorizontalTracks(canvas);
        }

        private void DrawVertical(ICanvas canvas)
        {
            canvas.Save();
            canvas.RotateDegrees(90, 50.0f, 50.0f);

            DrawHorizontal(canvas);

            canvas.Restore();
        }
        private void DrawCross(ICanvas canvas)
        {
            canvas.DrawPath(_horizontalPlankPath, _plankPaint);

            canvas.Save();
            canvas.RotateDegrees(90, 50.0f, 50.0f);

            DrawHorizontal(canvas);

            canvas.Restore();

            DrawHorizontalTracks(canvas);
        }

        private void DrawCorner(ICanvas canvas, Track track)
        {
            canvas.Save();
            canvas.RotateDegrees(track.Direction.TrackRotationAngle(), 50.0f, 50.0f);

            if (track.HasAlternateState() && track.AlternateState)
            {
                canvas.Scale(-1, 1);
                canvas.Translate(-100.0f, 0);
            }

            canvas.DrawPath(_cornerPlankPath, _plankPaint);

            if (track.HasAlternateState())
            {
                canvas.Save();
                canvas.RotateDegrees(90, 50.0f, 50.0f);

                canvas.DrawPath(_cornerSinglePlankPath, _plankPaint);

                canvas.ClipRect(new Rectangle(0, 0, 100.0f, 50.0f), false);

                DrawCornerTrack(canvas);

                canvas.Restore();
            }

            DrawCornerTrack(canvas);

            canvas.Restore();
        }

        private void DrawCornerTrack(ICanvas canvas)
        {
            canvas.DrawPath(_cornerTrackPath, _trackEdge);
            canvas.DrawPath(_cornerTrackPath, _trackClear);
        }

        private void DrawHorizontalTracks(ICanvas canvas)
        {
            canvas.DrawPath(_horizontalTrackPath, _trackEdge);
            canvas.DrawPath(_horizontalTrackPath, _trackClear);
        }
    }
}
