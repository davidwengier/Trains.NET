using System;

namespace Trains.NET.Engine
{
    public interface IMovable
    {
        Guid UniqueID { get; }
        float FrontEdgeDistance { get; }
        int Column { get; }
        int Row { get; }
        float Angle { get; }
        float RelativeLeft { get; }
        float RelativeTop { get; }
        void SetAngle(float angle);
    }
}
