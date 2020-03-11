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

        public void Deconstruct(out float relativeLeft, out float relativeTop, out float angle, out float distance)
        {
            relativeLeft = this.RelativeLeft;
            relativeTop = this.RelativeTop;
            angle = this.Angle;
            distance = this.Distance;
        }

        public static implicit operator (float RelativeLeft, float RelativeTop, float Angle, float Distance)(TrainPosition value)
        {
            return (value.RelativeLeft, value.RelativeTop, value.Angle, value.Distance);
        }

        public static implicit operator TrainPosition((float RelativeLeft, float RelativeTop, float Angle, float Distance) value)
        {
            return new TrainPosition(value.RelativeLeft, value.RelativeTop, value.Angle, value.Distance);
        }
    }
}
