namespace Trains.NET.Engine
{
    public class Train
    {
        public int Column { get; internal set; }
        public int Row { get; internal set; }

        public TrainDirection Direction { get; internal set; }

        internal void Move(int speedAdjustmentFactor, Track track)
        {
            var neighbors = track.GetNeighbors();
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
    }
}
