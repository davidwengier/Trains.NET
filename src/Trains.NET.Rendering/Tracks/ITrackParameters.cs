namespace Trains.NET.Rendering
{
    public interface ITrackParameters
    {
        int CellSize { get; set; }
        float PlankLength { get; set; }
        float PlankWidth { get; set; }
        int NumPlanks { get; set; }
        int NumCornerPlanks { get; set; }
        int TrackWidth { get; set; }
        float RailWidth { get; set; }
        float RailTopWidth { get; set; }
    }
}
