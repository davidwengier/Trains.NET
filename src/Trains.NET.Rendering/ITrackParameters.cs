namespace Trains.NET.Rendering
{
    public interface ITrackParameters
    {
        int CellSize { get; set; }
        int PlankWidth { get; set; }
        int NumPlanks { get; set; }
        int NumCornerPlanks { get; set; }
        int CornerEdgeOffsetDegrees { get; set; }
        int CornerStepDegrees { get; set; }
        int PlankPadding { get; set; }
        int TrackPadding { get; set; }
        int TrackWidth { get; set; }
    }
}
