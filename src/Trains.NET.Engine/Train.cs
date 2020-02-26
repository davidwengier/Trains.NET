using System;

namespace Trains.NET.Engine
{
    public class Train
    {
        private readonly Random _random = new Random();

        public int Column { get; internal set; }
        public int Row { get; internal set; }
        public float Angle { get; internal set; }
        public float RelativeLeft { get; internal set; } = 0.5f;
        public float RelativeTop { get; internal set; } = 0.5f;

        internal float Move(float distance, Track track)
        {
                float newLeft; 
                float newTop;
                int newCoumn = this.Column;
                int newRow = this.Row;

                (newLeft, newTop, this.Angle, distance) = 
                    track.Move(this.RelativeLeft, this.RelativeTop, this.Angle, distance);

                if (newLeft < 0.0f) 
                {
                    newCoumn--;
                    newLeft = 0.999f;
                }
                if (newLeft > 1.0f)
                {
                    newCoumn++;
                    newLeft = 0.001f;
                }
                if (newTop < 0.0f)
                {
                    newRow--;
                    newTop = 0.999f;
                }
                if (newTop > 1.0f)
                {
                    newRow++;
                    newTop = 0.001f;
                }

                // Wrap this in detection logic
                this.Column = newCoumn;
                this.Row = newRow;
                this.RelativeLeft = newLeft;
                this.RelativeTop = newTop;
            return distance;
        }
    }
}
