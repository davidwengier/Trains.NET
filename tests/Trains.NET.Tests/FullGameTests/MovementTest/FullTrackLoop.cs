using Trains.NET.Engine;
using static Trains.NET.Tests.TrainMovementTestsHelper;

namespace Trains.NET.Tests.FullGameTests.MovementTest;

public class FullTrackLoop_SingleStep(ITestOutputHelper output) : FullTrackLoop(output, 1)
{
}
public class FullTrackLoop_2Step(ITestOutputHelper output) : FullTrackLoop(output, 2)
{
}
public class FullTrackLoop_3Step(ITestOutputHelper output) : FullTrackLoop(output, 3)
{
}
public class FullTrackLoop_10Step(ITestOutputHelper output) : FullTrackLoop(output, 10)
{
}
public class FullTrackLoop_100Step(ITestOutputHelper output) : FullTrackLoop(output, 100)
{
}
public class FullTrackLoop_1000Step(ITestOutputHelper output) : FullTrackLoop(output, 1000)
{
}
public abstract class FullTrackLoop(ITestOutputHelper output, int movementSteps) : TestBase(output)
{
    private const float MovementPrecision = 3;
    private readonly int _movementSteps = movementSteps;

    [Theory]
    [InlineData(0.0f)]
    [InlineData(180.0f)]
    public void MovementTest_FullTrackLoop_3x3Square(float initialTrainAngle)
    {
        TrackLayout.AddTrack(1, 1);
        TrackLayout.AddTrack(2, 1);
        TrackLayout.AddTrack(3, 1);
        TrackLayout.AddTrack(3, 2);
        TrackLayout.AddTrack(3, 3);
        TrackLayout.AddTrack(2, 3);
        TrackLayout.AddTrack(1, 3);
        TrackLayout.AddTrack(1, 2);

        TrainManager.AddTrain(2, 1);

        var train = (Train)MovableLayout.Single();

        float distance = (float)(4 * StraightTrackDistance +
                                 4 * CornerTrackDistance);

        // Train speed & angle
        train.ForceSpeed(distance / _movementSteps / Train.SpeedScaleModifier);
        train.SetAngle(initialTrainAngle);

        Assert.Equal(2, train.Column);
        Assert.Equal(1, train.Row);
        Assert.Equal(0.5f, train.RelativeLeft, MovementPrecision);
        Assert.Equal(0.5f, train.RelativeTop, MovementPrecision);
        Assert.Equal(initialTrainAngle, train.Angle, MovementPrecision);

        // Move it!
        for (int i = 0; i < _movementSteps; i++)
            GameManager.GameLoopStep();

        Assert.Equal(2, train.Column);
        Assert.Equal(1, train.Row);
        Assert.Equal(0.5f, train.RelativeLeft, MovementPrecision);
        Assert.Equal(0.5f, train.RelativeTop, MovementPrecision);
        Assert.Equal(initialTrainAngle, train.Angle, MovementPrecision);
    }

    [Theory]
    [InlineData(0.0f)]
    [InlineData(180.0f)]
    public void MovementTest_FullTrackLoop_FourLoopCorners(float initialTrainAngle)
    {
        TrackLayout.AddTrack(1, 1, SingleTrackDirection.RightDown);
        TrackLayout.AddTrack(2, 1, SingleTrackDirection.LeftDown);
        TrackLayout.AddTrack(4, 1, SingleTrackDirection.RightDown);
        TrackLayout.AddTrack(5, 1, SingleTrackDirection.LeftDown);

        TrackLayout.AddTrack(1, 2, SingleTrackDirection.RightUp);
        TrackLayout.AddCrossTrack(2, 2);
        TrackLayout.AddTrack(3, 2, SingleTrackDirection.Horizontal);
        TrackLayout.AddCrossTrack(4, 2);
        TrackLayout.AddTrack(5, 2, SingleTrackDirection.LeftUp);

        TrackLayout.AddTrack(2, 3, SingleTrackDirection.Vertical);
        TrackLayout.AddTrack(4, 3, SingleTrackDirection.Vertical);

        TrackLayout.AddTrack(1, 4, SingleTrackDirection.RightDown);
        TrackLayout.AddCrossTrack(2, 4);
        TrackLayout.AddTrack(3, 4, SingleTrackDirection.Horizontal);
        TrackLayout.AddCrossTrack(4, 4);
        TrackLayout.AddTrack(5, 4, SingleTrackDirection.LeftDown);

        TrackLayout.AddTrack(1, 5, SingleTrackDirection.RightUp);
        TrackLayout.AddTrack(2, 5, SingleTrackDirection.LeftUp);
        TrackLayout.AddTrack(4, 5, SingleTrackDirection.RightUp);
        TrackLayout.AddTrack(5, 5, SingleTrackDirection.LeftUp);

        TrainManager.AddTrain(3, 2);

        var train = (Train)MovableLayout.Single();

        float distance = (float)(12 * StraightTrackDistance +
                                 12 * CornerTrackDistance);

        train.Angle = initialTrainAngle;
        train.ForceSpeed(distance / _movementSteps / Train.SpeedScaleModifier);

        Assert.Equal(3, train.Column);
        Assert.Equal(2, train.Row);
        Assert.Equal(0.5f, train.RelativeLeft, MovementPrecision);
        Assert.Equal(0.5f, train.RelativeTop, MovementPrecision);
        Assert.Equal(initialTrainAngle, train.Angle, MovementPrecision);

        // Move it!
        for (int i = 0; i < _movementSteps; i++)
            GameManager.GameLoopStep();

        Assert.Equal(3, train.Column);
        Assert.Equal(2, train.Row);
        Assert.Equal(0.5f, train.RelativeLeft, MovementPrecision);
        Assert.Equal(0.5f, train.RelativeTop, MovementPrecision);
        Assert.Equal(initialTrainAngle, train.Angle, MovementPrecision);
    }
}
