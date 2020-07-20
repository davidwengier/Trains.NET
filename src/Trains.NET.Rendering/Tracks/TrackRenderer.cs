using System;
using System.Collections.Generic;
using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    internal class TrackRenderer : ITrackRenderer
    {
        private readonly ITrackParameters _parameters;
        private readonly IBitmapFactory _bitmapFactory;
        private readonly IPathFactory _pathFactory;

        private readonly PaintBrush _trackEdge;
        private readonly PaintBrush _trackClear;
        private readonly PaintBrush _plankPaint;

        private readonly Dictionary<TrackDirection, IBitmap> _cachedTracks = new Dictionary<TrackDirection, IBitmap>();

        private readonly float _innerTrackOffset;
        private readonly float _outerTrackOffset;
        private readonly float _innerPlankOffset;
        private readonly float _outerPlankOffset;


        public TrackRenderer(ITrackParameters parameters, IBitmapFactory bitmapFactory, IPathFactory pathFactory)
        {
            _parameters = parameters;
            _bitmapFactory = bitmapFactory;
            _pathFactory = pathFactory;

            _innerTrackOffset = _parameters.CellSize / 2.0f - _parameters.TrackWidth / 2.0f;
            _outerTrackOffset = _parameters.CellSize / 2.0f + _parameters.TrackWidth / 2.0f;
            _innerPlankOffset = _parameters.CellSize / 2.0f - _parameters.PlankLength / 2.0f;
            _outerPlankOffset = _parameters.CellSize / 2.0f + _parameters.PlankLength / 2.0f;

            _plankPaint = new PaintBrush
            {
                Color = Colors.Black,
                Style = PaintStyle.Stroke,
                StrokeWidth = _parameters.PlankWidth
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
            if(!_cachedTracks.TryGetValue(track.Direction, out IBitmap cachedBitmap))
            {
                cachedBitmap = _bitmapFactory.CreateBitmap(_parameters.CellSize, _parameters.CellSize);
                ICanvas cachedCanvas = _bitmapFactory.CreateCanvas(cachedBitmap);

                DrawTrack(cachedCanvas, track.Direction);
            }
            canvas.DrawBitmap(cachedBitmap, 0, 0);
        }

        private void DrawTrack(ICanvas canvas, TrackDirection direction)
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
                    DrawCorner(canvas, direction);
                    break;
                case TrackDirection.Undefined:
                default:
                    break;
            }
        }

        private void DrawHorizontal(ICanvas canvas)
        {
            DrawHorizontalPlanks(canvas);
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
            DrawHorizontalPlanks(canvas);

            canvas.Save();
            canvas.RotateDegrees(90, _parameters.CellSize / 2, _parameters.CellSize / 2);

            DrawHorizontal(canvas);

            canvas.Restore();

            DrawHorizontalTracks(canvas);
        }

        private void DrawCorner(ICanvas canvas, TrackDirection direction)
        {
            float rotation = TrackAngle(direction);

            canvas.Save();
            canvas.RotateDegrees(rotation, _parameters.CellSize / 2, _parameters.CellSize / 2);
            
            DrawCornerPlanks(canvas);

            if(IsTrackThreeWay(direction))
            {
                canvas.Save();
                canvas.RotateDegrees(90, _parameters.CellSize / 2, _parameters.CellSize / 2);

                DrawCornerTrack(canvas);

                canvas.Restore();
            }

            DrawCornerTrack(canvas);

            canvas.Restore();
        }

        private void DrawCornerTrack(ICanvas canvas)
        {
            IPath? trackPath = _pathFactory.Create();

            trackPath.MoveTo(0, _innerTrackOffset);
            trackPath.QuadTo(_innerTrackOffset, _innerTrackOffset, _innerTrackOffset, 0);
            trackPath.MoveTo(0, _outerTrackOffset);
            trackPath.QuadTo(_outerTrackOffset, _outerTrackOffset, _outerTrackOffset, 0);

            canvas.DrawPath(trackPath, _trackEdge);
            canvas.DrawPath(trackPath, _trackClear);
        }

        private void DrawCornerPlanks(ICanvas canvas)
        {
            IPath? path = _pathFactory.Create();

            double step = Math.PI / 2.0 / (_parameters.NumPlanks + 1.0);

            for (int i = 0; i <= _parameters.NumPlanks; i++)
            {
                double angle = step * (i + 0.5f);

                float innerX = (float)(_innerPlankOffset * Math.Cos(angle));
                float innerY = (float)(_innerPlankOffset * Math.Sin(angle));
                float outerX = (float)(_outerPlankOffset * Math.Cos(angle));
                float outerY = (float)(_outerPlankOffset * Math.Sin(angle));

                path.MoveTo(innerX, innerY);
                path.LineTo(outerX, outerY);
            }

            canvas.DrawPath(path, _plankPaint);
        }

        private void DrawHorizontalTracks(ICanvas canvas)
        {
            // Draw under's

            IPath? trackPath = _pathFactory.Create();

            trackPath.MoveTo(0, _innerTrackOffset);
            trackPath.LineTo(_parameters.CellSize, _innerTrackOffset);
            trackPath.MoveTo(0, _outerTrackOffset);
            trackPath.LineTo(_parameters.CellSize, _outerTrackOffset);

            canvas.DrawPath(trackPath, _trackEdge);
            canvas.DrawPath(trackPath, _trackClear);
        }

        private void DrawHorizontalPlanks(ICanvas canvas)
        {
            float plankGap = _parameters.CellSize / _parameters.NumPlanks;

            IPath? path = _pathFactory.Create();

            for (int i = 1; i < _parameters.NumPlanks + 1; i++)
            {
                float pos = (i * plankGap) - (plankGap / 2);

                path.MoveTo(pos, _innerPlankOffset);
                path.LineTo(pos, _outerPlankOffset);
            }

            canvas.DrawPath(path, _plankPaint);
        }

        private static bool IsTrackThreeWay(TrackDirection direction) => direction == TrackDirection.RightUpDown ||
                                                            direction == TrackDirection.LeftRightDown ||
                                                            direction == TrackDirection.LeftUpDown ||
                                                            direction == TrackDirection.LeftRightUp;

        private static float TrackAngle(TrackDirection direction) => direction switch
        {
            TrackDirection.LeftUp => 0,
            TrackDirection.LeftRightUp => 0,

            TrackDirection.RightUp => 90,
            TrackDirection.RightUpDown => 90,

            TrackDirection.RightDown => 180,
            TrackDirection.LeftRightDown => 180,

            TrackDirection.LeftDown => 270,
            TrackDirection.LeftUpDown => 270,

            _ => 0
        };
    }
}
