namespace Trains.NET.Engine
{
    public class CrossTrack : Track
    {
        public override void Move(TrainPosition position)
        {
            if ((position.Angle > 45.0f && position.Angle < 135.0f) ||
                (position.Angle > 225.0f && position.Angle < 315.0f))
            {
                TrainMovement.MoveVertical(position);
            }
            else
            {
                TrainMovement.MoveHorizontal(position);
            }
        }

        public override bool IsConnectedDown() => true;
        public override bool IsConnectedUp() => true;
        public override bool IsConnectedLeft() => true;
        public override bool IsConnectedRight() => true;
    }
}
