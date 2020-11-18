using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    [Order(100)]
    public class SingleTrackRenderer : SpecializedEntityRenderer<SingleTrack, Track>
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

        public SingleTrackRenderer(ITrackParameters trackParameters, ITrackPathBuilder trackPathBuilder)
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

        protected override void Render(ICanvas canvas, SingleTrack track)
        {
            DrawSingleTrack(canvas, track);
        }

        public void DrawSingleTrack(ICanvas canvas, SingleTrack track)
        {
            switch (track.Direction)
            {
                case SingleTrackDirection.Undefined:
                case SingleTrackDirection.Horizontal:
                    DrawHorizontal(canvas);
                    break;
                case SingleTrackDirection.Vertical:
                    DrawVertical(canvas);
                    break;
                case SingleTrackDirection.LeftUp:
                case SingleTrackDirection.RightUp:
                case SingleTrackDirection.RightDown:
                case SingleTrackDirection.LeftDown:
                    DrawCorner(canvas, track);
                    break;
                default:
                    break;
            }
        }

        private void DrawHorizontal(ICanvas canvas)
        {
            DrawHorizontalPlankPath(canvas);
            DrawHorizontalTracks(canvas);
        }

        public void DrawHorizontalPlankPath(ICanvas canvas)
        {
            canvas.DrawPath(_horizontalPlankPath, _plankPaint);
        }

        private void DrawVertical(ICanvas canvas)
        {
            using (canvas.Scope())
            {
                canvas.RotateDegrees(90, 50.0f, 50.0f);

                DrawHorizontal(canvas);

            }
        }
        public void DrawCross(ICanvas canvas)
        {
            canvas.DrawPath(_horizontalPlankPath, _plankPaint);

            using (canvas.Scope())
            {
                canvas.RotateDegrees(90, 50.0f, 50.0f);

                DrawHorizontal(canvas);
            }

            DrawHorizontalTracks(canvas);
        }

        private void DrawCorner(ICanvas canvas, SingleTrack track)
        {
            using (canvas.Scope())
            {
                var rotationAndgle = track.Direction switch
                {
                    SingleTrackDirection.LeftUp => 0,
                    SingleTrackDirection.RightUp => 90,
                    SingleTrackDirection.RightDown => 180,
                    SingleTrackDirection.LeftDown => 270,
                    _ => 0
                };

                canvas.RotateDegrees(rotationAndgle, 50.0f, 50.0f);

                DrawCornerPlankPath(canvas, false);

                DrawCornerTrack(canvas);
            }
        }

        public void DrawCornerPlankPath(ICanvas canvas, bool singlePlank)
        {
            canvas.DrawPath(singlePlank ? _cornerSinglePlankPath : _cornerPlankPath, _plankPaint);
        }

        public void DrawCornerTrack(ICanvas canvas)
        {
            canvas.DrawPath(_cornerTrackPath, _trackEdge);
            canvas.DrawPath(_cornerTrackPath, _trackClear);
        }

        public void DrawHorizontalTracks(ICanvas canvas)
        {
            canvas.DrawPath(_horizontalTrackPath, _trackEdge);
            canvas.DrawPath(_horizontalTrackPath, _trackClear);
        }
    }
}
