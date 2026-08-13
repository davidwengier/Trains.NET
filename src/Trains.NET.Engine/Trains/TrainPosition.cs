namespace Trains.NET.Engine;

public class TrainPosition(float relativeLeft, float relativeTop, float angle, float distance)
{
    public float RelativeLeft { get; set; } = relativeLeft;
    public float RelativeTop { get; set; } = relativeTop;
    public float Angle { get; set; } = angle;
    public float Distance { get; set; } = distance;
    public int Column { get; internal set; }
    public int Row { get; internal set; }

    public TrainPosition(int column, int row, float relativeLeft, float relativeTop, float angle, float distance)
        : this(relativeLeft, relativeTop, angle, distance)
    {
        Column = column;
        Row = row;
    }

    internal TrainPosition Clone() => new(Column, Row, RelativeLeft, RelativeTop, Angle, Distance);
}
