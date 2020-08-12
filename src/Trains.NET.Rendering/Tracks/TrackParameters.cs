namespace Trains.NET.Rendering
{
    public class TrackParameters : ITrackParameters
    {
        public int NumPlanks { get; set; }
        public int NumCornerPlanks { get; set; }
        public float PlankLength { get; set; }
        public float PlankWidth { get; set; }
        public int TrackWidth { get; set; }
        public float RailWidth { get; set; }
        public float RailTopWidth { get; set; }

        public TrackParameters()
        {
            this.PlankLength = 26;
            this.PlankWidth = 4.0f;
            this.NumPlanks = 3;
            this.NumCornerPlanks = this.NumPlanks + 1;
            this.TrackWidth = 12;
            this.RailWidth = 4f;
            this.RailTopWidth = 2.75f;
        }
    }
}
