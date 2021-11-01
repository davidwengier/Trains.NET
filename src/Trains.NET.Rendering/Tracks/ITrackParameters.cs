namespace Trains.NET.Rendering;

public interface ITrackParameters
{
    float PlankLength { get; }
    float PlankWidth { get; }
    int NumPlanks { get; }
    int NumCornerPlanks { get; }
    float TrackWidth { get; }
    float RailWidth { get; }
    float RailTopWidth { get; }
}
