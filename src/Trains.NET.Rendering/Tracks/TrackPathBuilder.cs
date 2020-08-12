using System;

namespace Trains.NET.Rendering
{
    public class TrackPathBuilder : ITrackPathBuilder
    {
        private readonly float _innerTrackOffset;
        private readonly float _outerTrackOffset;
        private readonly float _innerPlankOffset;
        private readonly float _outerPlankOffset;
        private readonly IGameParameters _gameParameters;
        private readonly ITrackParameters _trackParameters;
        private readonly IPathFactory _pathFactory;

        public TrackPathBuilder(IGameParameters gameParameters, ITrackParameters trackParameters, IPathFactory pathFactory)
        {
            _gameParameters = gameParameters;
            _trackParameters = trackParameters;
            _pathFactory = pathFactory;

            _innerTrackOffset = _gameParameters.CellSize / 2.0f - _trackParameters.TrackWidth / 2.0f;
            _outerTrackOffset = _gameParameters.CellSize / 2.0f + _trackParameters.TrackWidth / 2.0f;
            _innerPlankOffset = _gameParameters.CellSize / 2.0f - _trackParameters.PlankLength / 2.0f;
            _outerPlankOffset = _gameParameters.CellSize / 2.0f + _trackParameters.PlankLength / 2.0f;
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
            trackPath.LineTo(_gameParameters.CellSize, _innerTrackOffset);
            trackPath.MoveTo(0, _outerTrackOffset);
            trackPath.LineTo(_gameParameters.CellSize, _outerTrackOffset);

            return trackPath;
        }

        public IPath BuildHorizontalPlankPath()
        {
            float plankGap = _gameParameters.CellSize / _trackParameters.NumPlanks;

            IPath path = _pathFactory.Create();

            for (int i = 1; i < _trackParameters.NumPlanks + 1; i++)
            {
                float pos = (i * plankGap) - (plankGap / 2);

                path.MoveTo(pos, _innerPlankOffset);
                path.LineTo(pos, _outerPlankOffset);
            }

            return path;
        }

        public IPath BuildCornerPlankPath() => BuildCornerPlankPath(_trackParameters.NumCornerPlanks);

        public IPath BuildCornerPlankPath(int plankCount)
        {
            IPath path = _pathFactory.Create();

            double step = Math.PI / 2.0 / _trackParameters.NumCornerPlanks;

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
}
