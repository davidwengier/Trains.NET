namespace Trains.NET.Rendering
{
    public class TrackParameters : ITrackParameters
    {
        public int NumPlanks { get; } = 3;
        public int NumCornerPlanks => this.NumPlanks + 1;

        public float PlankLength { get; } = 65;
        public float PlankWidth { get; } = 10.0f;
        public float TrackWidth { get; } = 30.0f;
        public float RailWidth { get; } = 10.0f;
        public float RailTopWidth { get; } = 6.875f;
    }
}
