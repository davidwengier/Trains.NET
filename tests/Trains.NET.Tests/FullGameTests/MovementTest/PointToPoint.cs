using System.Linq;
using Trains.NET.Engine;
using Xunit;
using Xunit.Abstractions;
using static Trains.NET.Tests.TrainMovementTestsHelper;

namespace Trains.NET.Tests.FullGameTests.MovementTest;

public class PointToPoint_SingleStep : PointToPoint
{
    public PointToPoint_SingleStep(ITestOutputHelper output) : base(output, 1) { }
}
public class PointToPoint_2Step : PointToPoint
{
    public PointToPoint_2Step(ITestOutputHelper output) : base(output, 2) { }
}
public class PointToPoint_3Step : PointToPoint
{
    public PointToPoint_3Step(ITestOutputHelper output) : base(output, 3) { }
}
public class PointToPoint_10Step : PointToPoint
{
    public PointToPoint_10Step(ITestOutputHelper output) : base(output, 10) { }
}
public class PointToPoint_100Step : PointToPoint
{
    public PointToPoint_100Step(ITestOutputHelper output) : base(output, 100) { }
}
public class PointToPoint_1000Step : PointToPoint
{
    public PointToPoint_1000Step(ITestOutputHelper output) : base(output, 1000) { }
}
public abstract class PointToPoint : TestBase
{
    private readonly int _movementSteps;
    private const int MovementPrecision = 4;

    public PointToPoint(ITestOutputHelper output, int movementSteps)
        : base(output)
    {
        _movementSteps = movementSteps;
    }

    [Theory]
    [InlineData(1, 1, 90.0f, 1, 3)]
    [InlineData(1, 3, 270.0f, 1, 1)]
    public void MovementTest_PointToPoint_3VerticalTracks(int startingColumn, int startingRow, float angle, int expectedColumn, int expectedRow)
    {
        TrackLayout.AddTrack(1, 1);
        TrackLayout.AddTrack(1, 2);
        TrackLayout.AddTrack(1, 3);

        TrainManager.AddTrain(startingColumn, startingRow);

        float distance = (float)(HalfStraightTrackDistance + StraightTrackDistance + HalfStraightTrackDistance);

        var train = (Train)MovableLayout.Get().Single();
        train.ForceSpeed(distance / _movementSteps / Train.SpeedScaleModifier);
        train.Angle = angle;
        // We have an edge coming up, disable lookahead
        train.LookaheadDistance = 0.0f;

        Assert.Equal(startingColumn, train.Column);
        Assert.Equal(startingRow, train.Row);
        Assert.Equal(0.5f, train.RelativeLeft, MovementPrecision);
        Assert.Equal(0.5f, train.RelativeTop, MovementPrecision);
        Assert.Equal(angle, train.Angle, MovementPrecision);

        // Move it!
        for (int i = 0; i < _movementSteps; i++)
            GameBoard.GameLoopStep();

        Assert.Equal(expectedColumn, train.Column);
        Assert.Equal(expectedRow, train.Row);
        Assert.Equal(0.5f, train.RelativeLeft, MovementPrecision);
        Assert.Equal(0.5f, train.RelativeTop, MovementPrecision);
        Assert.Equal(angle, train.Angle, MovementPrecision);
    }

    [Theory]
    [InlineData(1, 1, 0.0f, 3, 1)]
    [InlineData(3, 1, 180.0f, 1, 1)]
    public void MovementTest_PointToPoint_3HorizontalTracks(int startingColumn, int startingRow, float angle, int expectedColumn, int expectedRow)
    {
        TrackLayout.AddTrack(1, 1);
        TrackLayout.AddTrack(2, 1);
        TrackLayout.AddTrack(3, 1);

        TrainManager.AddTrain(startingColumn, startingRow);

        float distance = (float)(HalfStraightTrackDistance + StraightTrackDistance + HalfStraightTrackDistance);

        var train = (Train)MovableLayout.Get().Single();
        train.ForceSpeed(distance / _movementSteps / Train.SpeedScaleModifier);
        train.Angle = angle;
        // We have an edge coming up, disable lookahead
        train.LookaheadDistance = 0.0f;

        Assert.Equal(startingColumn, train.Column);
        Assert.Equal(startingRow, train.Row);
        Assert.Equal(0.5f, train.RelativeLeft, MovementPrecision);
        Assert.Equal(0.5f, train.RelativeTop, MovementPrecision);
        Assert.Equal(angle, train.Angle, MovementPrecision);

        // Move it!
        for (int i = 0; i < _movementSteps; i++)
            GameBoard.GameLoopStep();

        Assert.Equal(expectedColumn, train.Column);
        Assert.Equal(expectedRow, train.Row);
        Assert.Equal(0.5f, train.RelativeLeft, MovementPrecision);
        Assert.Equal(0.5f, train.RelativeTop, MovementPrecision);
        Assert.Equal(angle, train.Angle, MovementPrecision);
    }

    [Theory]
    [InlineData(1, 2, 270.0f, 2, 1, 0.0f)]
    [InlineData(2, 1, 180.0f, 1, 2, 90.0f)]
    public void MovementTest_PointToPoint_Vertical_RightDown_Horizontal(int startingColumn, int startingRow, float startingAngle, int expectedColumn, int expectedRow, float expectedAngle)
    {
        TrackLayout.AddTrack(1, 2); // Vertical
        TrackLayout.AddTrack(1, 1); // Corner
        TrackLayout.AddTrack(2, 1); // Horizontal

        TrainManager.AddTrain(startingColumn, startingRow);

        float distance = (float)(HalfStraightTrackDistance + CornerTrackDistance + HalfStraightTrackDistance);

        var train = (Train)MovableLayout.Get().Single();
        train.ForceSpeed(distance / _movementSteps / Train.SpeedScaleModifier);
        train.Angle = startingAngle;
        // We have an edge coming up, disable lookahead
        train.LookaheadDistance = 0.0f;

        Assert.Equal(startingColumn, train.Column);
        Assert.Equal(startingRow, train.Row);
        Assert.Equal(0.5f, train.RelativeLeft, MovementPrecision);
        Assert.Equal(0.5f, train.RelativeTop, MovementPrecision);
        Assert.Equal(startingAngle, train.Angle, MovementPrecision);

        // Move it!
        for (int i = 0; i < _movementSteps; i++)
            GameBoard.GameLoopStep();

        Assert.Equal(expectedColumn, train.Column);
        Assert.Equal(expectedRow, train.Row);
        Assert.Equal(0.5f, train.RelativeLeft, MovementPrecision);
        Assert.Equal(0.5f, train.RelativeTop, MovementPrecision);
        Assert.Equal(expectedAngle, train.Angle, MovementPrecision);
    }

    [Theory]
    [InlineData(1, 1, 90.0f, 2, 2, 0.0f)]
    [InlineData(2, 2, 180.0f, 1, 1, 270.0f)]
    public void MovementTest_PointToPoint_Horizontal_RightUp_Vertical(int startingColumn, int startingRow, float startingAngle, int expectedColumn, int expectedRow, float expectedAngle)
    {
        TrackLayout.AddTrack(1, 1); // Vertical
        TrackLayout.AddTrack(1, 2); // Corner
        TrackLayout.AddTrack(2, 2); // Horizontal

        TrainManager.AddTrain(startingColumn, startingRow);

        float distance = (float)(HalfStraightTrackDistance + CornerTrackDistance + HalfStraightTrackDistance);

        var train = (Train)MovableLayout.Get().Single();
        train.ForceSpeed(distance / _movementSteps / Train.SpeedScaleModifier);
        train.Angle = startingAngle;
        // We have an edge coming up, disable lookahead
        train.LookaheadDistance = 0.0f;

        Assert.Equal(startingColumn, train.Column);
        Assert.Equal(startingRow, train.Row);
        Assert.Equal(0.5f, train.RelativeLeft, MovementPrecision);
        Assert.Equal(0.5f, train.RelativeTop, MovementPrecision);
        Assert.Equal(startingAngle, train.Angle, MovementPrecision);

        // Move it!
        for (int i = 0; i < _movementSteps; i++)
            GameBoard.GameLoopStep();

        Assert.Equal(expectedColumn, train.Column);
        Assert.Equal(expectedRow, train.Row);
        Assert.Equal(0.5f, train.RelativeLeft, MovementPrecision);
        Assert.Equal(0.5f, train.RelativeTop, MovementPrecision);
        Assert.Equal(expectedAngle, train.Angle, MovementPrecision);
    }

    [Theory]
    [InlineData(1, 1, 0.0f, 2, 2, 90.0)]
    [InlineData(2, 2, 270.0f, 1, 1, 180.0f)]
    public void MovementTest_PointToPoint_Horizontal_LeftDown_Vertical(int startingColumn, int startingRow, float startingAngle, int expectedColumn, int expectedRow, float expectedAngle)
    {
        TrackLayout.AddTrack(1, 1); // Horizontal
        TrackLayout.AddTrack(2, 1); // Corner
        TrackLayout.AddTrack(2, 2); // Vertical

        TrainManager.AddTrain(startingColumn, startingRow);

        float distance = (float)(HalfStraightTrackDistance + CornerTrackDistance + HalfStraightTrackDistance);

        var train = (Train)MovableLayout.Get().Single();
        train.ForceSpeed(distance / _movementSteps / Train.SpeedScaleModifier);
        train.Angle = startingAngle;
        // We have an edge coming up, disable lookahead
        train.LookaheadDistance = 0.0f;

        Assert.Equal(startingColumn, train.Column);
        Assert.Equal(startingRow, train.Row);
        Assert.Equal(0.5f, train.RelativeLeft, MovementPrecision);
        Assert.Equal(0.5f, train.RelativeTop, MovementPrecision);
        Assert.Equal(startingAngle, train.Angle, MovementPrecision);

        // Move it!
        for (int i = 0; i < _movementSteps; i++)
            GameBoard.GameLoopStep();

        Assert.Equal(expectedColumn, train.Column);
        Assert.Equal(expectedRow, train.Row);
        Assert.Equal(0.5f, train.RelativeLeft, MovementPrecision);
        Assert.Equal(0.5f, train.RelativeTop, MovementPrecision);
        Assert.Equal(expectedAngle, train.Angle, MovementPrecision);
    }

    [Theory]
    [InlineData(1, 2, 0.0f, 2, 1, 270.0f)]
    [InlineData(2, 1, 90.0f, 1, 2, 180.0f)]
    public void MovementTest_PointToPoint_Horizontal_LeftUp_Vertical(int startingColumn, int startingRow, float startingAngle, int expectedColumn, int expectedRow, float expectedAngle)
    {
        TrackLayout.AddTrack(1, 2); // Horizontal
        TrackLayout.AddTrack(2, 2); // Corner
        TrackLayout.AddTrack(2, 1); // Vertical

        TrainManager.AddTrain(startingColumn, startingRow);

        float distance = (float)(HalfStraightTrackDistance + CornerTrackDistance + HalfStraightTrackDistance);

        var train = (Train)MovableLayout.Get().Single();
        train.ForceSpeed(distance / _movementSteps / Train.SpeedScaleModifier);
        train.Angle = startingAngle;
        // We have an edge coming up, disable lookahead
        train.LookaheadDistance = 0.0f;

        Assert.Equal(startingColumn, train.Column);
        Assert.Equal(startingRow, train.Row);
        Assert.Equal(0.5f, train.RelativeLeft, MovementPrecision);
        Assert.Equal(0.5f, train.RelativeTop, MovementPrecision);
        Assert.Equal(startingAngle, train.Angle, MovementPrecision);

        // Move it!
        for (int i = 0; i < _movementSteps; i++)
            GameBoard.GameLoopStep();

        Assert.Equal(expectedColumn, train.Column);
        Assert.Equal(expectedRow, train.Row);
        Assert.Equal(0.5f, train.RelativeLeft, MovementPrecision);
        Assert.Equal(0.5f, train.RelativeTop, MovementPrecision);
        Assert.Equal(expectedAngle, train.Angle, MovementPrecision);
    }

    [Theory]
    [InlineData(2, 2, 90.0f, 2, 4)] // Down
    [InlineData(2, 4, 270.0f, 2, 2)] // Up
    [InlineData(1, 3, 0.0f, 3, 3)] // Right
    [InlineData(3, 3, 180.0f, 1, 3)] // Left
    public void MovementTest_PointToPoint_HorizontalVertical_Cross_HorizontalVertical(int startingColumn, int startingRow, float angle, int expectedColumn, int expectedRow)
    {
        TrackTool.Execute(2, 1, new ExecuteInfo(0, 0)); // Vertical
        TrackTool.Execute(2, 2, new ExecuteInfo(2, 1)); // Vertical
        TrackTool.Execute(1, 3, new ExecuteInfo(2, 2)); // Horizontal
        TrackTool.Execute(3, 3, new ExecuteInfo(1, 3)); // Horizontal
        TrackTool.Execute(2, 4, new ExecuteInfo(3, 3)); // Vertical
        TrackTool.Execute(2, 5, new ExecuteInfo(2, 4)); // Vertical
        TrackTool.Execute(2, 3, new ExecuteInfo(2, 5)); // Cross

        TrainManager.AddTrain(startingColumn, startingRow);

        float distance = (float)(HalfStraightTrackDistance + StraightTrackDistance + HalfStraightTrackDistance);

        var train = (Train)MovableLayout.Get().Single();
        train.ForceSpeed(distance / _movementSteps / Train.SpeedScaleModifier);
        train.Angle = angle;
        // We have an edge coming up, disable lookahead
        train.LookaheadDistance = 0.0f;

        Assert.Equal(startingColumn, train.Column);
        Assert.Equal(startingRow, train.Row);
        Assert.Equal(0.5f, train.RelativeLeft, MovementPrecision);
        Assert.Equal(0.5f, train.RelativeTop, MovementPrecision);
        Assert.Equal(angle, train.Angle, MovementPrecision);

        // Move it!
        for (int i = 0; i < _movementSteps; i++)
            GameBoard.GameLoopStep();

        Assert.Equal(expectedColumn, train.Column);
        Assert.Equal(expectedRow, train.Row);
        Assert.Equal(0.5f, train.RelativeLeft, MovementPrecision);
        Assert.Equal(0.5f, train.RelativeTop, MovementPrecision);
        Assert.Equal(angle, train.Angle, MovementPrecision);
    }

    [Theory]
    [InlineData(1, 2, 0.0f, 2, 3, 90.0f)] // Left to Down
    [InlineData(2, 1, 90.0f, 1, 2, 180.0f)] // Up to Left
    [InlineData(2, 3, 270.0f, 1, 2, 180.0f)] // Down to Left
    public void MovementTest_PointToPoint_Horizontal_LeftUpDown_VerticalVertical(int startingColumn, int startingRow, float startingAngle, int expectedColumn, int expectedRow, float expectedAngle)
    {
        TrackLayout.AddTrack(1, 2, SingleTrackDirection.Horizontal);
        TrackLayout.AddTrack(2, 1, SingleTrackDirection.Vertical);
        TrackLayout.AddTrack(2, 3, SingleTrackDirection.Vertical);
        TrackLayout.AddTIntersectionTrack(2, 2, TIntersectionDirection.LeftDown_LeftUp, TIntersectionStyle.CornerAndPrimary);

        TrainManager.AddTrain(startingColumn, startingRow);

        float distance = (float)(HalfStraightTrackDistance + CornerTrackDistance + HalfStraightTrackDistance);

        var train = (Train)MovableLayout.Get().Single();
        train.ForceSpeed(distance / _movementSteps / Train.SpeedScaleModifier);
        train.Angle = startingAngle;
        // We have an edge coming up, disable lookaheadÏ
        train.LookaheadDistance = 0.0f;

        Assert.Equal(startingColumn, train.Column);
        Assert.Equal(startingRow, train.Row);
        Assert.Equal(0.5f, train.RelativeLeft, MovementPrecision);
        Assert.Equal(0.5f, train.RelativeTop, MovementPrecision);
        Assert.Equal(startingAngle, train.Angle, MovementPrecision);

        // Move it!
        for (int i = 0; i < _movementSteps; i++)
            GameBoard.GameLoopStep();

        Assert.Equal(expectedColumn, train.Column);
        Assert.Equal(expectedRow, train.Row);
        Assert.Equal(0.5f, train.RelativeLeft, MovementPrecision);
        Assert.Equal(0.5f, train.RelativeTop, MovementPrecision);
        Assert.Equal(expectedAngle, train.Angle, MovementPrecision);
    }

    [Theory]
    [InlineData(2, 2, 180.0f, 1, 1, 270.0f)] // Right to Up
    [InlineData(1, 1, 90.0f, 2, 2, 0.0f)] // Up to Right
    [InlineData(1, 3, 270.0f, 2, 2, 0.0f)] // Down to Right
    public void MovementTest_PointToPoint_VerticalVertical_RightUpDown_Horizontal(int startingColumn, int startingRow, float startingAngle, int expectedColumn, int expectedRow, float expectedAngle)
    {
        TrackLayout.AddTrack(1, 1, SingleTrackDirection.Vertical);
        TrackLayout.AddTrack(1, 3, SingleTrackDirection.Vertical);
        TrackLayout.AddTrack(2, 2, SingleTrackDirection.Horizontal);
        TrackLayout.AddTIntersectionTrack(1, 2, TIntersectionDirection.RightUp_RightDown, TIntersectionStyle.CornerAndPrimary);

        TrainManager.AddTrain(startingColumn, startingRow);

        float distance = (float)(HalfStraightTrackDistance + CornerTrackDistance + HalfStraightTrackDistance);

        var train = (Train)MovableLayout.Get().Single();
        train.ForceSpeed(distance / _movementSteps / Train.SpeedScaleModifier);
        train.Angle = startingAngle;
        // We have an edge coming up, disable lookahead
        train.LookaheadDistance = 0.0f;

        Assert.Equal(startingColumn, train.Column);
        Assert.Equal(startingRow, train.Row);
        Assert.Equal(0.5f, train.RelativeLeft, MovementPrecision);
        Assert.Equal(0.5f, train.RelativeTop, MovementPrecision);
        Assert.Equal(startingAngle, train.Angle, MovementPrecision);

        // Move it!
        for (int i = 0; i < _movementSteps; i++)
            GameBoard.GameLoopStep();

        Assert.Equal(expectedColumn, train.Column);
        Assert.Equal(expectedRow, train.Row);
        Assert.Equal(0.5f, train.RelativeLeft, MovementPrecision);
        Assert.Equal(0.5f, train.RelativeTop, MovementPrecision);
        Assert.Equal(expectedAngle, train.Angle, MovementPrecision);
    }

    [Theory]
    [InlineData(1, 1, 0.0f, 2, 2, 90.0f)] // Left to Down
    [InlineData(2, 2, 270.0f, 3, 1, 0.0f)] // Down to Left
    [InlineData(3, 1, 180.0f, 2, 2, 90.0f)] // Right to Down
    public void MovementTest_PointToPoint_HorizontalHorizontal_LeftRightDown_Vertical(int startingColumn, int startingRow, float startingAngle, int expectedColumn, int expectedRow, float expectedAngle)
    {
        TrackLayout.AddTrack(1, 1, SingleTrackDirection.Horizontal);
        TrackLayout.AddTrack(3, 1, SingleTrackDirection.Horizontal);
        TrackLayout.AddTrack(2, 2, SingleTrackDirection.Vertical);
        TrackLayout.AddTIntersectionTrack(2, 1, TIntersectionDirection.RightDown_LeftDown, TIntersectionStyle.CornerAndPrimary);

        TrainManager.AddTrain(startingColumn, startingRow);

        float distance = (float)(HalfStraightTrackDistance + CornerTrackDistance + HalfStraightTrackDistance);

        var train = (Train)MovableLayout.Get().Single();
        train.ForceSpeed(distance / _movementSteps / Train.SpeedScaleModifier);
        train.Angle = startingAngle;
        // We have an edge coming up, disable lookahead
        train.LookaheadDistance = 0.0f;

        Assert.Equal(startingColumn, train.Column);
        Assert.Equal(startingRow, train.Row);
        Assert.Equal(0.5f, train.RelativeLeft, MovementPrecision);
        Assert.Equal(0.5f, train.RelativeTop, MovementPrecision);
        Assert.Equal(startingAngle, train.Angle, MovementPrecision);

        // Move it!
        for (int i = 0; i < _movementSteps; i++)
            GameBoard.GameLoopStep();

        Assert.Equal(expectedColumn, train.Column);
        Assert.Equal(expectedRow, train.Row);
        Assert.Equal(0.5f, train.RelativeLeft, MovementPrecision);
        Assert.Equal(0.5f, train.RelativeTop, MovementPrecision);
        Assert.Equal(expectedAngle, train.Angle, MovementPrecision);
    }

    [Theory]
    [InlineData(1, 2, 0.0f, 2, 1, 270.0f)] // Left to Up
    [InlineData(2, 1, 90.0f, 1, 2, 180.0f)] // Up to Left
    [InlineData(3, 2, 180.0f, 2, 1, 270.0f)] // Right to Up
    public void MovementTest_PointToPoint_HorizontalHorizontal_LeftRightUp_Vertical(int startingColumn, int startingRow, float startingAngle, int expectedColumn, int expectedRow, float expectedAngle)
    {
        TrackLayout.AddTrack(1, 2, SingleTrackDirection.Horizontal);
        TrackLayout.AddTrack(3, 2, SingleTrackDirection.Horizontal);
        TrackLayout.AddTrack(2, 1, SingleTrackDirection.Vertical);
        TrackLayout.AddTIntersectionTrack(2, 2, TIntersectionDirection.LeftUp_RightUp, TIntersectionStyle.CornerAndPrimary);

        TrainManager.AddTrain(startingColumn, startingRow);

        float distance = (float)(HalfStraightTrackDistance + CornerTrackDistance + HalfStraightTrackDistance);

        var train = (Train)MovableLayout.Get().Single();
        train.ForceSpeed(distance / _movementSteps / Train.SpeedScaleModifier);
        train.Angle = startingAngle;
        // We have an edge coming up, disable lookahead
        train.LookaheadDistance = 0.0f;

        Assert.Equal(startingColumn, train.Column);
        Assert.Equal(startingRow, train.Row);
        Assert.Equal(0.5f, train.RelativeLeft, MovementPrecision);
        Assert.Equal(0.5f, train.RelativeTop, MovementPrecision);
        Assert.Equal(startingAngle, train.Angle, MovementPrecision);

        // Move it!
        for (int i = 0; i < _movementSteps; i++)
            GameBoard.GameLoopStep();

        Assert.Equal(expectedColumn, train.Column);
        Assert.Equal(expectedRow, train.Row);
        Assert.Equal(0.5f, train.RelativeLeft, MovementPrecision);
        Assert.Equal(0.5f, train.RelativeTop, MovementPrecision);
        Assert.Equal(expectedAngle, train.Angle, MovementPrecision);
    }
}
