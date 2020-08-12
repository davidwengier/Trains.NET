using System.Collections.Generic;
using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    public class TrackRenderer : IRenderer<Track>
    {
        private readonly ITrackParameters _trackParameters;
        private readonly IGameParameters _gameParameters;
        private readonly IImageFactory _imageFactory;

        private readonly PaintBrush _trackEdge;
        private readonly PaintBrush _trackClear;
        private readonly PaintBrush _plankPaint;

        private readonly Dictionary<string, IImage> _cachedTracks = new Dictionary<string, IImage>();

        private readonly IPath _cornerTrackPath;
        private readonly IPath _cornerPlankPath;
        private readonly IPath _cornerSinglePlankPath;
        private readonly IPath _horizontalTrackPath;
        private readonly IPath _horizontalPlankPath;

        public TrackRenderer(ITrackParameters trackParameters, IGameParameters gameParameters, IImageFactory imageFactory, ITrackPathBuilder trackPathBuilder)
        {
            _trackParameters = trackParameters;
            _gameParameters = gameParameters;
            _imageFactory = imageFactory;

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

        public void Render(ICanvas canvas, Track track)
        {
            if (!_cachedTracks.TryGetValue(track.Identifier, out IImage cachedImage))
            {
                using IImageCanvas? imageCanvas = _imageFactory.CreateImageCanvas(_gameParameters.CellSize, _gameParameters.CellSize);

                DrawTrack(imageCanvas.Canvas, track.Direction, track);

                cachedImage = imageCanvas.Render();
                _cachedTracks[track.Identifier] = cachedImage;
            }
            canvas.DrawImage(cachedImage, 0, 0);
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
            canvas.RotateDegrees(90, _gameParameters.CellSize / 2, _gameParameters.CellSize / 2);

            DrawHorizontal(canvas);

            canvas.Restore();
        }
        private void DrawCross(ICanvas canvas)
        {
            canvas.DrawPath(_horizontalPlankPath, _plankPaint);

            canvas.Save();
            canvas.RotateDegrees(90, _gameParameters.CellSize / 2, _gameParameters.CellSize / 2);

            DrawHorizontal(canvas);

            canvas.Restore();

            DrawHorizontalTracks(canvas);
        }

        private void DrawCorner(ICanvas canvas, TrackDirection direction, Track track)
        {
            canvas.Save();
            canvas.RotateDegrees(direction.TrackRotationAngle(), _gameParameters.CellSize / 2, _gameParameters.CellSize / 2);

            if (track.HasAlternateState() && track.AlternateState)
            {
                canvas.Scale(-1, 1);
                canvas.Translate(-_gameParameters.CellSize, 0);
            }

            canvas.DrawPath(_cornerPlankPath, _plankPaint);

            if (track.HasAlternateState())
            {
                canvas.Save();
                canvas.RotateDegrees(90, _gameParameters.CellSize / 2, _gameParameters.CellSize / 2);

                canvas.DrawPath(_cornerSinglePlankPath, _plankPaint);

                canvas.ClipRect(new Rectangle(0, 0, _gameParameters.CellSize, _gameParameters.CellSize / 2), ClipOperation.Intersect, false);

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
