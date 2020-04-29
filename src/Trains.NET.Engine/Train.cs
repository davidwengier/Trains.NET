using System;

namespace Trains.NET.Engine
{
    public class Train : IMovable
    {
        public float FrontEdgeDistance { get; internal set; } = 0.8f;

        public Guid UniqueID { get; internal set; } = Guid.NewGuid();
        public int Column { get; internal set; }
        public int Row { get; internal set; }
        public float Angle { get; internal set; }
        public float RelativeLeft { get; internal set; } = 0.5f;
        public float RelativeTop { get; internal set; } = 0.5f;

        public void SetAngle(float angle)
        {
            this.Angle = angle;
        }

        internal Train Clone()
        {
            return new Train()
            {
                UniqueID = this.UniqueID,
                Column = this.Column,
                Row = this.Row,
                Angle = this.Angle,
                RelativeLeft = this.RelativeLeft,
                RelativeTop = this.RelativeTop
            };
        }
    }
}
