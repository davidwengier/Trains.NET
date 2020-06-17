namespace Trains.NET.Rendering
{
    public class TrackParameters : ITrackParameters
    {
        public int CellSize { get; set; }
        public int PlankWidth { get; set; }
        public int NumPlanks { get; set; }
        public int NumCornerPlanks { get; set; }
        public int CornerEdgeOffsetDegrees { get; set; }
        public int CornerStepDegrees { get; set; }
        public int PlankPadding { get; set; }
        public int TrackPadding { get; set; }
        public int TrackWidth { get; set; }

        public TrackParameters()
        {
            this.CellSize = 40;
            this.PlankWidth = 3;
            this.NumPlanks = 3;
            this.NumCornerPlanks = this.NumPlanks + 1;
            this.CornerEdgeOffsetDegrees = 10;
            this.CornerStepDegrees =
                 // Initial angle to draw is 90 degrees, but CornerStepDegrees is only for the middle planks
                 // so remove the first and last from the swept angle
                 (90 - 2 * this.CornerEdgeOffsetDegrees)
                 // Now just split up the remainder amongst the middle planks
                 / (this.NumCornerPlanks - 1);
            this.PlankPadding = 5;
            this.TrackPadding = 10;
            this.TrackWidth = 4;
        }
    }
}
