namespace Trains.NET.Rendering;

public class TrackPathBuilder : ITrackPathBuilder
{
    private readonly float _innerTrackOffset;
    private readonly float _outerTrackOffset;
    private readonly float _innerPlankOffset;
    private readonly float _outerPlankOffset;
    private readonly ITrackParameters _trackParameters;
    private readonly IPathFactory _pathFactory;

    public TrackPathBuilder(ITrackParameters trackParameters, IPathFactory pathFactory)
    {
        _trackParameters = trackParameters;
        _pathFactory = pathFactory;

        _innerTrackOffset = 50.0f - _trackParameters.TrackWidth / 2.0f;
        _outerTrackOffset = 50.0f + _trackParameters.TrackWidth / 2.0f;
        _innerPlankOffset = 50.0f - _trackParameters.PlankLength / 2.0f;
        _outerPlankOffset = 50.0f + _trackParameters.PlankLength / 2.0f;
    }
    public IPath BuildCornerTrackPath()
    {
        var trackPath = _pathFactory.Create();

        trackPath.MoveTo(0, _innerTrackOffset);
        trackPath.ConicTo(_innerTrackOffset, _innerTrackOffset, _innerTrackOffset, 0, 0.75f);
        trackPath.MoveTo(0, _outerTrackOffset);
        trackPath.ConicTo(_outerTrackOffset, _outerTrackOffset, _outerTrackOffset, 0, 0.75f);

        return trackPath;
    }

    public IPath BuildHorizontalTrackPath()
    {
        var trackPath = _pathFactory.Create();

        trackPath.MoveTo(0, _innerTrackOffset);
        trackPath.LineTo(100.0f, _innerTrackOffset);
        trackPath.MoveTo(0, _outerTrackOffset);
        trackPath.LineTo(100.0f, _outerTrackOffset);

        return trackPath;
    }

    public IPath BuildHorizontalPlankPath()
    {
        var plankGap = 100.0f / _trackParameters.NumPlanks;

        var path = _pathFactory.Create();

        for (var i = 1; i < _trackParameters.NumPlanks + 1; i++)
        {
            var pos = (i * plankGap) - (plankGap / 2);

            path.MoveTo(pos, _innerPlankOffset);
            path.LineTo(pos, _outerPlankOffset);
        }

        return path;
    }

    public IPath BuildCornerPlankPath() => BuildCornerPlankPath(_trackParameters.NumCornerPlanks);

    public IPath BuildCornerPlankPath(int plankCount)
    {
        var path = _pathFactory.Create();

        var step = Math.PI / 2.0 / _trackParameters.NumCornerPlanks;

        for (var i = 0; i < plankCount; i++)
        {
            var angle = step * (i + 0.5f);

            var innerX = (float)(_innerPlankOffset * Math.Cos(angle));
            var innerY = (float)(_innerPlankOffset * Math.Sin(angle));
            var outerX = (float)(_outerPlankOffset * Math.Cos(angle));
            var outerY = (float)(_outerPlankOffset * Math.Sin(angle));

            path.MoveTo(innerX, innerY);
            path.LineTo(outerX, outerY);
        }

        return path;
    }
}
