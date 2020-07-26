using System.Collections.Generic;
using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    public class TrackRenderer : ITrackRenderer
    {
        private bool _cacheBitmaps = false;

        private readonly ITrackParameters _parameters;
        private readonly IBitmapFactory _bitmapFactory;

        private readonly PaintBrush _trackEdge;
        private readonly PaintBrush _trackClear;
        private readonly PaintBrush _plankPaint;

        private readonly Dictionary<TrackDirection, IBitmap> _cachedTracks = new Dictionary<TrackDirection, IBitmap>();

        private readonly IPath _cornerTrackPath;
        private readonly IPath _cornerPlankPath;
        private readonly IPath _cornerSinglePlankPath;
        private readonly IPath _horizontalTrackPath;
        private readonly IPath _horizontalPlankPath;

        public TrackRenderer(ITrackParameters parameters, IBitmapFactory bitmapFactory, ITrackPathBuilder trackPathBuilder)
        {
            _parameters = parameters;
            _bitmapFactory = bitmapFactory;

            _cornerTrackPath = trackPathBuilder.BuildCornerTrackPath();
            _cornerPlankPath = trackPathBuilder.BuildCornerPlankPath();
            _cornerSinglePlankPath = trackPathBuilder.BuildCornerPlankPath(1);
            _horizontalTrackPath = trackPathBuilder.BuildHorizontalTrackPath();
            _horizontalPlankPath = trackPathBuilder.BuildHorizontalPlankPath();

            _plankPaint = new PaintBrush
            {
                Color = Colors.Black,
                Style = PaintStyle.Stroke,
                StrokeWidth = _parameters.PlankWidth,
                IsAntialias = true
            };
            _trackClear = new PaintBrush
            {
                Color = Colors.White,
                Style = PaintStyle.Stroke,
                StrokeWidth = _parameters.RailTopWidth,
                IsAntialias = true
            };
            _trackEdge = new PaintBrush
            {
                Color = Colors.Black,
                Style = PaintStyle.Stroke,
                StrokeWidth = _parameters.RailWidth,
                IsAntialias = true
            };
        }

        public void Render(ICanvas canvas, Track track)
        {
            if (_cacheBitmaps)
            {
                if (!_cachedTracks.TryGetValue(track.Direction, out IBitmap cachedBitmap))
                {
                    cachedBitmap = _bitmapFactory.CreateBitmap(_parameters.CellSize, _parameters.CellSize);
                    ICanvas cachedCanvas = _bitmapFactory.CreateCanvas(cachedBitmap);

                    DrawTrack(cachedCanvas, track.Direction, track);
                }
                canvas.DrawBitmap(cachedBitmap, 0, 0);
            }
            else
            {
                DrawTrack(canvas, track.Direction, track);
            }
        }

        private void DrawTrack(ICanvas canvas, TrackDirection direction, Track track)
        {
            switch (direction)
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
                case TrackDirection.RightUpDown:
                case TrackDirection.LeftRightDown:
                case TrackDirection.LeftUpDown:
                case TrackDirection.LeftRightUp:
                    DrawCorner(canvas, direction, track);
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
            canvas.RotateDegrees(90, _parameters.CellSize / 2, _parameters.CellSize / 2);

            DrawHorizontal(canvas);

            canvas.Restore();
        }
        private void DrawCross(ICanvas canvas)
        {
            canvas.DrawPath(_horizontalPlankPath, _plankPaint);

            canvas.Save();
            canvas.RotateDegrees(90, _parameters.CellSize / 2, _parameters.CellSize / 2);

            DrawHorizontal(canvas);

            canvas.Restore();

            DrawHorizontalTracks(canvas);
        }

        private void DrawCorner(ICanvas canvas, TrackDirection direction, Track track)
        {
            canvas.Save();
            canvas.RotateDegrees(direction.TrackRotationAngle(), _parameters.CellSize / 2, _parameters.CellSize / 2);

            if (track.HasAlternateState() && track.AlternateState)
            {
                canvas.Scale(-1, 1);
                canvas.Translate(-_parameters.CellSize, 0);
            }

            canvas.DrawPath(_cornerPlankPath, _plankPaint);

            if (track.HasAlternateState())
            {
                canvas.Save();
                canvas.RotateDegrees(90, _parameters.CellSize / 2, _parameters.CellSize / 2);

                canvas.DrawPath(_cornerSinglePlankPath, _plankPaint);

                canvas.ClipRect(new Rectangle(0, 0, _parameters.CellSize, _parameters.CellSize / 2), ClipOperation.Intersect, false);

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
