namespace Trains.NET.Rendering
{
    internal class TrackParameters : ITrackParameters
    {
        public int CellSize { get; set; }
        
        public int NumPlanks { get; set; }
        public int NumCornerPlanks { get; set; }

        public float PlankLength { get; set; }
        public float PlankWidth { get; set; }
        public int TrackWidth { get; set; }
        public float RailWidth { get; set; }
        public float RailTopWidth { get; set; }

        public TrackParameters()
        {
            this.CellSize = 40;
            this.PlankLength = 26;
            this.PlankWidth = 4.0f;
            this.NumPlanks = 3;
            this.NumCornerPlanks = this.NumPlanks + 1;
            this.TrackWidth = 12;
            this.RailWidth = 5f;
            this.RailTopWidth = 4.25f;
        }
    }
}
