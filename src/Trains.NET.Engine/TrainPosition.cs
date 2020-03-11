namespace Trains.NET.Engine
{
    public class TrainPosition
    {
        public float RelativeLeft { get; set; }
        public float RelativeTop { get; set; }
        public float Angle { get; set; }
        public float Distance { get; set; }

        public TrainPosition(float relativeLeft, float relativeTop, float angle, float distance)
        {
            this.RelativeLeft = relativeLeft;
            this.RelativeTop = relativeTop;
            this.Angle = angle;
            this.Distance = distance;
        }
    }
}
