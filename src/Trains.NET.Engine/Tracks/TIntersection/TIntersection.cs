using System;

namespace Trains.NET.Engine
{
    public class TIntersection : Track
    {
        public override bool HasMultipleStates => true;

        public bool AlternateState { get; set; }

        public TIntersectionDirection Direction { get; set; }

        public override string Identifier => $"{this.Direction}.{this.AlternateState}";


        public override void NextState()
        {
            this.AlternateState = !this.AlternateState;

            OnChanged();
        }

        public override void Move(TrainPosition position)
        {
            switch (this.Direction)
            {
                case TIntersectionDirection.RightUp_RightDown: MoveRightUpDown(position); break;
                case TIntersectionDirection.RightDown_LeftDown: MoveLeftRightDown(position); break;
                case TIntersectionDirection.LeftDown_LeftUp: MoveLeftUpDown(position); break;
                case TIntersectionDirection.LeftUp_RightUp: MoveLeftRightUp(position); break;
                default: throw new InvalidOperationException("I don't know what that track is!");
            }
        }

        public override bool IsConnectedRight()
            => this.Direction switch
            {
                TIntersectionDirection.RightDown_LeftDown => true,
                TIntersectionDirection.LeftUp_RightUp => true,
                TIntersectionDirection.RightUp_RightDown => true,
                _ => false
            };

        public override bool IsConnectedDown()
            => this.Direction switch
            {
                TIntersectionDirection.RightDown_LeftDown => true,
                TIntersectionDirection.LeftDown_LeftUp => true,
                TIntersectionDirection.RightUp_RightDown => true,
                _ => false
            };

        public override bool IsConnectedLeft()
            => this.Direction switch
            {
                TIntersectionDirection.RightDown_LeftDown => true,
                TIntersectionDirection.LeftUp_RightUp => true,
                TIntersectionDirection.LeftDown_LeftUp => true,
                _ => false
            };

        public override bool IsConnectedUp()
            => this.Direction switch
            {
                TIntersectionDirection.LeftDown_LeftUp => true,
                TIntersectionDirection.LeftUp_RightUp => true,
                TIntersectionDirection.RightUp_RightDown => true,
                _ => false
            };

        private void MoveLeftRightDown(TrainPosition position)
        {
            // Check single track extremes, as there are 2 places where the
            //  train angle could be at 0 degrees
            if (position.RelativeLeft < 0.4f)
            {
                TrainMovement.MoveLeftDown(position);
            }
            else if (position.RelativeLeft > 0.6f)
            {
                TrainMovement.MoveRightDown(position);
            }
            else if (position.Angle <= 90.0)
            {
                TrainMovement.MoveLeftDown(position);
            }
            else
            {
                if (this.AlternateState)
                {
                    TrainMovement.MoveLeftDown(position);
                }
                else
                {
                    TrainMovement.MoveRightDown(position);
                }
            }
        }

        private void MoveLeftUpDown(TrainPosition position)
        {
            if (position.RelativeTop < 0.4f)
            {
                TrainMovement.MoveLeftUp(position);
            }
            else if (position.RelativeTop > 0.6f)
            {
                TrainMovement.MoveLeftDown(position);
            }
            else if (TrainMovement.BetweenAngles(position.Angle, 89, 181))
            {
                TrainMovement.MoveLeftUp(position);
            }
            else
            {
                if (this.AlternateState)
                {
                    TrainMovement.MoveLeftUp(position);
                }
                else
                {
                    TrainMovement.MoveLeftDown(position);
                }
            }
        }

        private void MoveLeftRightUp(TrainPosition position)
        {
            if (position.RelativeLeft < 0.4f)
            {
                TrainMovement.MoveLeftUp(position);
            }
            else if (position.RelativeLeft > 0.6f)
            {
                TrainMovement.MoveRightUp(position);
            }
            else if (TrainMovement.BetweenAngles(position.Angle, 179, 271))
            {
                TrainMovement.MoveRightUp(position);
            }
            else
            {
                if (this.AlternateState)
                {
                    TrainMovement.MoveRightUp(position);
                }
                else
                {
                    TrainMovement.MoveLeftUp(position);
                }
            }
        }

        private void MoveRightUpDown(TrainPosition position)
        {
            // Right -> Up, Enters 180, Leaves 270
            // Up -> Right, Enters 90, Leaves 0
            // Down -> Right, Enters 270, Leaves 0
            if (position.RelativeTop < 0.4f)
            {
                TrainMovement.MoveRightUp(position);
            }
            else if (position.RelativeTop > 0.6f)
            {
                TrainMovement.MoveRightDown(position);
            }
            else if (position.Angle >= 270.0)
            {
                TrainMovement.MoveRightDown(position);
            }
            else
            {
                if (this.AlternateState)
                {
                    TrainMovement.MoveRightDown(position);
                }
                else
                {
                    TrainMovement.MoveRightUp(position);
                }
            }
        }
    }
}
