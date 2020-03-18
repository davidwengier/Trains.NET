namespace Trains.NET.Engine
{
    public interface IMovable
    {
        float FrontEdgeDistance { get; }
        int Column { get; }
        int Row { get; }
        float Angle { get; }
        float RelativeLeft { get; }
        float RelativeTop { get; }
    }
}