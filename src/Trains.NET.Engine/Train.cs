using System;

namespace Trains.NET.Engine
{
    public class Train
    {
        private readonly Random _random = new Random();

        public int Column { get; internal set; }
        public int Row { get; internal set; }

        public TrainDirection Direction { get; internal set; }

        internal void Move(int speedAdjustmentFactor, Track track)
        {
            TrackNeighbors neighbors = track.GetNeighbors();
            Track? nextTrack = null;
            if (this.Direction == TrainDirection.Left && neighbors.Left != null)
            {
                this.Column -= speedAdjustmentFactor;
                nextTrack = neighbors.Left;
            }
            else if (this.Direction == TrainDirection.Right && neighbors.Right != null)
            {
                this.Column += speedAdjustmentFactor;
                nextTrack = neighbors.Right;
            }
            else if (this.Direction == TrainDirection.Up && neighbors.Up != null)
            {
                this.Row -= speedAdjustmentFactor;
                nextTrack = neighbors.Up;
            }
            else if (this.Direction == TrainDirection.Down && neighbors.Down != null)
            {
                this.Row += speedAdjustmentFactor;
                nextTrack = neighbors.Down;
            }

            if (nextTrack != null)
            {
                this.Direction = nextTrack.GetTrainDirection(this.Direction);
            }
        }

        internal void SetBestDirection(Track track)
        {
            TrainDirection direction = track.Direction switch
            {
                TrackDirection.Vertical => Randomize(TrainDirection.Up, TrainDirection.Down),
                TrackDirection.Horizontal => Randomize(TrainDirection.Left, TrainDirection.Right),
                TrackDirection.LeftUp => Randomize(TrainDirection.Left, TrainDirection.Up),
                TrackDirection.RightUp => Randomize(TrainDirection.Right, TrainDirection.Up),
                TrackDirection.RightDown => Randomize(TrainDirection.Right, TrainDirection.Down),
                TrackDirection.LeftDown => Randomize(TrainDirection.Left, TrainDirection.Down),
                TrackDirection.RightUpDown => Randomize(TrainDirection.Right, TrainDirection.Up, TrainDirection.Down),
                TrackDirection.LeftRightDown => Randomize(TrainDirection.Left, TrainDirection.Right, TrainDirection.Down),
                TrackDirection.LeftUpDown => Randomize(TrainDirection.Left, TrainDirection.Up, TrainDirection.Down),
                TrackDirection.LeftRightUp => Randomize(TrainDirection.Left, TrainDirection.Right, TrainDirection.Up),
                TrackDirection.Cross => Randomize(TrainDirection.Up, TrainDirection.Down, TrainDirection.Left, TrainDirection.Right),
                _ => throw new InvalidOperationException()
            };
            this.Direction = direction;
        }

        private TrainDirection Randomize(params TrainDirection[] possibleDirections) => possibleDirections[_random.Next(0, possibleDirections.Length)];
    }
}
