using System;

namespace Trains.NET.Rendering
{
    public class TrackPathBuilder : ITrackPathBuilder
    {
        private readonly float _innerTrackOffset;
        private readonly float _outerTrackOffset;
        private readonly float _innerPlankOffset;
        private readonly float _outerPlankOffset;
        private readonly ITrackParameters _parameters;
        private readonly IPathFactory _pathFactory;

        public TrackPathBuilder(ITrackParameters parameters, IPathFactory pathFactory)
        {
            _parameters = parameters;
            _pathFactory = pathFactory;

            _innerTrackOffset = _parameters.CellSize / 2.0f - _parameters.TrackWidth / 2.0f;
            _outerTrackOffset = _parameters.CellSize / 2.0f + _parameters.TrackWidth / 2.0f;
            _innerPlankOffset = _parameters.CellSize / 2.0f - _parameters.PlankLength / 2.0f;
            _outerPlankOffset = _parameters.CellSize / 2.0f + _parameters.PlankLength / 2.0f;
        }
        public IPath BuildCornerTrackPath()
        {
            IPath trackPath = _pathFactory.Create();

            trackPath.MoveTo(0, _innerTrackOffset);
            trackPath.QuadTo(_innerTrackOffset, _innerTrackOffset, _innerTrackOffset, 0);
            trackPath.MoveTo(0, _outerTrackOffset);
            trackPath.QuadTo(_outerTrackOffset, _outerTrackOffset, _outerTrackOffset, 0);

            return trackPath;
        }

        public IPath BuildHorizontalTrackPath()
        {
            IPath trackPath = _pathFactory.Create();

            trackPath.MoveTo(0, _innerTrackOffset);
            trackPath.LineTo(_parameters.CellSize, _innerTrackOffset);
            trackPath.MoveTo(0, _outerTrackOffset);
            trackPath.LineTo(_parameters.CellSize, _outerTrackOffset);

            return trackPath;
        }

        public IPath BuildHorizontalPlankPath()
        {
            float plankGap = _parameters.CellSize / _parameters.NumPlanks;

            IPath path = _pathFactory.Create();

            for (int i = 1; i < _parameters.NumPlanks + 1; i++)
            {
                float pos = (i * plankGap) - (plankGap / 2);

                path.MoveTo(pos, _innerPlankOffset);
                path.LineTo(pos, _outerPlankOffset);
            }

            return path;
        }

        public IPath BuildCornerPlankPath() => BuildCornerPlankPath(_parameters.NumCornerPlanks);

        public IPath BuildCornerPlankPath(int plankCount)
        {
            IPath path = _pathFactory.Create();

            double step = Math.PI / 2.0 / _parameters.NumCornerPlanks;

            for (int i = 0; i < plankCount; i++)
            {
                double angle = step * (i + 0.5f);

                float innerX = (float)(_innerPlankOffset * Math.Cos(angle));
                float innerY = (float)(_innerPlankOffset * Math.Sin(angle));
                float outerX = (float)(_outerPlankOffset * Math.Cos(angle));
                float outerY = (float)(_outerPlankOffset * Math.Sin(angle));

                path.MoveTo(innerX, innerY);
                path.LineTo(outerX, outerY);
            }

            return path;
        }
    }
    public interface ITrackPathBuilder
    {
        IPath BuildHorizontalTrackPath();
        IPath BuildHorizontalPlankPath();
        IPath BuildCornerTrackPath();
        IPath BuildCornerPlankPath();
        IPath BuildCornerPlankPath(int plankCount);
    }
}
