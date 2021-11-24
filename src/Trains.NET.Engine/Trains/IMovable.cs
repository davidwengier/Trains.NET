namespace Trains.NET.Engine;

public interface IMovable : IEntity
{
    Guid UniqueID { get; }
    float LookaheadDistance { get; }
    float Angle { get; }
    float RelativeLeft { get; }
    float RelativeTop { get; }
    bool Follow { get; }

    void SetAngle(float angle);
}
