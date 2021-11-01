namespace Trains.NET.Engine;

public class TrainPosition
{
    public float RelativeLeft { get; set; }
    public float RelativeTop { get; set; }
    public float Angle { get; set; }
    public float Distance { get; set; }
    public int Column { get; internal set; }
    public int Row { get; internal set; }

    public TrainPosition(float relativeLeft, float relativeTop, float angle, float distance)
    {
        this.RelativeLeft = relativeLeft;
        this.RelativeTop = relativeTop;
        this.Angle = angle;
        this.Distance = distance;
    }

    public TrainPosition(int column, int row, float relativeLeft, float relativeTop, float angle, float distance)
        : this(relativeLeft, relativeTop, angle, distance)
    {
        this.Column = column;
        this.Row = row;
    }

    internal TrainPosition Clone() => new(this.Column, this.Row, this.RelativeLeft, this.RelativeTop, this.Angle, this.Distance);
}
