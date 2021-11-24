namespace Trains.NET.Engine;

public class TIntersection : Track
{
    public override bool HasMultipleStates => true;

    public TIntersectionStyle Style { get; set; }

    public TIntersectionDirection Direction { get; set; }

    public override string Identifier => $"{this.Direction}.{this.Style}";

    public override void NextState()
    {
        this.Style = this.Style + 1;

        if (this.Style > TIntersectionStyle.StraightAndSecondary)
        {
            this.Style = TIntersectionStyle.CornerAndPrimary;
        }

        OnChanged();
    }

    public override void Move(TrainPosition position)
    {
        switch (this.Direction)
        {
            case TIntersectionDirection.RightUp_RightDown: MoveRightUp_RightDown(position); break;
            case TIntersectionDirection.RightDown_LeftDown: MoveRightDown_LeftDown(position); break;
            case TIntersectionDirection.LeftDown_LeftUp: MoveLeftDown_LeftUp(position); break;
            case TIntersectionDirection.LeftUp_RightUp: MoveLeftUp_RightUp(position); break;
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

    private void MoveRightDown_LeftDown(TrainPosition position)
    {
        if (this.Style is TIntersectionStyle.StraightAndPrimary or TIntersectionStyle.StraightAndSecondary)
        {
            if (position.Angle is 0 or 180)
            {
                TrainMovement.MoveHorizontal(position);
            }
            else if (this.Style is TIntersectionStyle.StraightAndPrimary)
            {
                TrainMovement.MoveLeftDown(position);
            }
            else
            {
                TrainMovement.MoveRightDown(position);
            }
        }
        else if (position.RelativeLeft < 0.4f)
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
            if (this.Style is TIntersectionStyle.CornerAndSecondary)
            {
                TrainMovement.MoveLeftDown(position);
            }
            else
            {
                TrainMovement.MoveRightDown(position);
            }
        }
    }

    private void MoveLeftDown_LeftUp(TrainPosition position)
    {
        if (this.Style is TIntersectionStyle.StraightAndPrimary or TIntersectionStyle.StraightAndSecondary)
        {
            if (position.Angle is 90 or 270)
            {
                TrainMovement.MoveVertical(position);
            }
            else if (this.Style is TIntersectionStyle.StraightAndPrimary)
            {
                TrainMovement.MoveLeftUp(position);
            }
            else
            {
                TrainMovement.MoveLeftDown(position);
            }
        }
        else if (position.RelativeTop < 0.4f)
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
            if (this.Style == TIntersectionStyle.CornerAndSecondary)
            {
                TrainMovement.MoveLeftUp(position);
            }
            else
            {
                TrainMovement.MoveLeftDown(position);
            }
        }
    }

    private void MoveLeftUp_RightUp(TrainPosition position)
    {
        if (this.Style is TIntersectionStyle.StraightAndPrimary or TIntersectionStyle.StraightAndSecondary)
        {
            if (position.Angle is 0 or 180)
            {
                TrainMovement.MoveHorizontal(position);
            }
            else if (this.Style is TIntersectionStyle.StraightAndPrimary)
            {
                TrainMovement.MoveRightUp(position);
            }
            else
            {
                TrainMovement.MoveLeftUp(position);
            }
        }
        else if (position.RelativeLeft < 0.4f)
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
            if (this.Style == TIntersectionStyle.CornerAndSecondary)
            {
                TrainMovement.MoveRightUp(position);
            }
            else
            {
                TrainMovement.MoveLeftUp(position);
            }
        }
    }

    private void MoveRightUp_RightDown(TrainPosition position)
    {
        if (this.Style is TIntersectionStyle.StraightAndPrimary or TIntersectionStyle.StraightAndSecondary)
        {
            if (position.Angle is 90 or 270)
            {
                TrainMovement.MoveVertical(position);
            }
            else if (this.Style is TIntersectionStyle.StraightAndPrimary)
            {
                TrainMovement.MoveRightDown(position);
            }
            else
            {
                TrainMovement.MoveRightUp(position);
            }
        }
        else if (position.RelativeTop < 0.4f)
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
            if (this.Style == TIntersectionStyle.CornerAndSecondary)
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
