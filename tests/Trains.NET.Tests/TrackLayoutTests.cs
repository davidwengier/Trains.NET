using Trains.NET.Engine;

namespace Trains.NET.Tests;

public class TrackLayoutTests(ITestOutputHelper output) : TestBase(output)
{
    [Fact]
    public void FishHook()
    {
        TrackTool.Execute(5, 2, new ExecuteInfo(0, 0));
        TrackTool.Execute(4, 2, new ExecuteInfo(5, 2));
        TrackTool.Execute(3, 2, new ExecuteInfo(4, 2));
        TrackTool.Execute(3, 3, new ExecuteInfo(3, 2));
        TrackTool.Execute(2, 3, new ExecuteInfo(3, 3));
        TrackTool.Execute(1, 3, new ExecuteInfo(2, 3));
        TrackTool.Execute(1, 2, new ExecuteInfo(1, 3));
        TrackTool.Execute(1, 1, new ExecuteInfo(1, 2));
        TrackTool.Execute(2, 1, new ExecuteInfo(1, 1));
        TrackTool.Execute(3, 1, new ExecuteInfo(2, 1));

        Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(5, 2).Direction);
        Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(4, 2).Direction);
        Assert.Equal(SingleTrackDirection.RightDown, TrackLayout.GetTrackAt<SingleTrack>(3, 2).Direction);
        Assert.Equal(SingleTrackDirection.LeftUp, TrackLayout.GetTrackAt<SingleTrack>(3, 3).Direction);
        Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(2, 3).Direction);
        Assert.Equal(SingleTrackDirection.RightUp, TrackLayout.GetTrackAt<SingleTrack>(1, 3).Direction);
        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(1, 2).Direction);
        Assert.Equal(SingleTrackDirection.RightDown, TrackLayout.GetTrackAt<SingleTrack>(1, 1).Direction);
        Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(2, 1).Direction);
        Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(3, 1).Direction);
    }

    [Fact]
    public void VerticalHairpin()
    {
        TrackTool.Execute(1, 3, new ExecuteInfo(0, 0));
        TrackTool.Execute(1, 2, new ExecuteInfo(1, 3));
        TrackTool.Execute(1, 1, new ExecuteInfo(1, 2));
        TrackTool.Execute(2, 1, new ExecuteInfo(1, 1));
        TrackTool.Execute(2, 2, new ExecuteInfo(2, 1));

        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(1, 3).Direction);
        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(1, 2).Direction);
        Assert.Equal(SingleTrackDirection.RightDown, TrackLayout.GetTrackAt<SingleTrack>(1, 1).Direction);
        Assert.Equal(SingleTrackDirection.LeftDown, TrackLayout.GetTrackAt<SingleTrack>(2, 1).Direction);
        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(2, 2).Direction);
    }

    [Fact]
    public void SidewaysHairpin()
    {
        TrackTool.Execute(1, 2, new ExecuteInfo(0, 0));
        TrackTool.Execute(2, 2, new ExecuteInfo(1, 2));
        TrackTool.Execute(3, 2, new ExecuteInfo(2, 2));
        TrackTool.Execute(3, 1, new ExecuteInfo(3, 2));
        TrackTool.Execute(2, 1, new ExecuteInfo(3, 1));

        Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(1, 2).Direction);
        Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(2, 2).Direction);
        Assert.Equal(SingleTrackDirection.LeftUp, TrackLayout.GetTrackAt<SingleTrack>(3, 2).Direction);
        Assert.Equal(SingleTrackDirection.LeftDown, TrackLayout.GetTrackAt<SingleTrack>(3, 1).Direction);
        Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(2, 1).Direction);
    }

    [Fact]
    public void CrossVerticalFirst()
    {
        TrackTool.Execute(2, 1, new ExecuteInfo(0, 0));
        TrackTool.Execute(2, 2, new ExecuteInfo(2, 1));
        TrackTool.Execute(2, 3, new ExecuteInfo(2, 2));
        TrackTool.Execute(1, 2, new ExecuteInfo(0, 0));
        TrackTool.Execute(2, 2, new ExecuteInfo(1, 2));
        TrackTool.Execute(3, 2, new ExecuteInfo(2, 2));

        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(2, 1).Direction);
        Assert.NotNull(TrackLayout.GetTrackAt<CrossTrack>(2, 2));
        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(2, 3).Direction);
        Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(1, 2).Direction);
        Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(3, 2).Direction);
    }

    [Fact]
    public void CrossHorizontalFirst()
    {
        TrackTool.Execute(1, 2, new ExecuteInfo(0, 0));
        TrackTool.Execute(2, 2, new ExecuteInfo(1, 2));
        TrackTool.Execute(3, 2, new ExecuteInfo(2, 2));
        TrackTool.Execute(2, 1, new ExecuteInfo(0, 0));
        TrackTool.Execute(2, 2, new ExecuteInfo(2, 1));
        TrackTool.Execute(2, 3, new ExecuteInfo(2, 2));

        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(2, 1).Direction);
        Assert.NotNull(TrackLayout.GetTrackAt<CrossTrack>(2, 2));
        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(2, 3).Direction);
        Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(1, 2).Direction);
        Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(3, 2).Direction);
    }

    [Fact]
    public void CrossHortizontalFirstDrawDown()
    {
        TrackTool.Execute(1, 2, new ExecuteInfo(0, 0));
        TrackTool.Execute(2, 2, new ExecuteInfo(1, 2));
        TrackTool.Execute(3, 2, new ExecuteInfo(2, 2));
        TrackTool.Execute(2, 1, new ExecuteInfo(0, 0));
        TrackTool.Execute(2, 2, new ExecuteInfo(2, 1));
        TrackTool.Execute(2, 3, new ExecuteInfo(2, 2));

        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(2, 1).Direction);
        Assert.NotNull(TrackLayout.GetTrackAt<CrossTrack>(2, 2));
        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(2, 3).Direction);
        Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(1, 2).Direction);
        Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(3, 2).Direction);
    }

    [Fact]
    public void CrossHortizontalFirstDrawUp()
    {
        TrackTool.Execute(1, 2, new ExecuteInfo(0, 0));
        TrackTool.Execute(2, 2, new ExecuteInfo(1, 2));
        TrackTool.Execute(3, 2, new ExecuteInfo(2, 2));
        TrackTool.Execute(2, 3, new ExecuteInfo(0, 0));
        TrackTool.Execute(2, 2, new ExecuteInfo(2, 3));
        TrackTool.Execute(2, 1, new ExecuteInfo(2, 2));

        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(2, 1).Direction);
        Assert.NotNull(TrackLayout.GetTrackAt<CrossTrack>(2, 2));
        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(2, 3).Direction);
        Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(1, 2).Direction);
        Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(3, 2).Direction);
    }

    [Fact]
    public void CrossMiddleLast()
    {
        TrackTool.Execute(1, 3, new ExecuteInfo(0, 0));
        TrackTool.Execute(2, 1, new ExecuteInfo(1, 3));
        TrackTool.Execute(2, 2, new ExecuteInfo(2, 1));
        TrackTool.Execute(3, 3, new ExecuteInfo(2, 2));
        TrackTool.Execute(2, 4, new ExecuteInfo(3, 3));
        TrackTool.Execute(2, 5, new ExecuteInfo(2, 4));

        TrackTool.Execute(2, 3, new ExecuteInfo(0, 0));

        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(2, 2).Direction);
        Assert.NotNull(TrackLayout.GetTrackAt<CrossTrack>(2, 3));
        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(2, 4).Direction);
        Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(1, 3).Direction);
        Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(3, 3).Direction);
    }

    [Fact]
    public void TwoCrosses_DragLeft()
    {
        TrackTool.Execute(2, 1, new ExecuteInfo(0, 0));
        TrackTool.Execute(2, 2, new ExecuteInfo(3, 1));
        TrackTool.Execute(2, 3, new ExecuteInfo(2, 2));

        TrackTool.Execute(4, 1, new ExecuteInfo(0, 0));
        TrackTool.Execute(4, 2, new ExecuteInfo(4, 1));
        TrackTool.Execute(4, 3, new ExecuteInfo(4, 2));

        TrackTool.Execute(1, 2, new ExecuteInfo(0, 0));
        TrackTool.Execute(2, 2, new ExecuteInfo(1, 2));
        TrackTool.Execute(3, 2, new ExecuteInfo(2, 2));
        TrackTool.Execute(4, 2, new ExecuteInfo(3, 2));
        TrackTool.Execute(5, 2, new ExecuteInfo(4, 2));

        Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(1, 2).Direction);
        Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(3, 2).Direction);
        Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(5, 2).Direction);

        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(2, 1).Direction);
        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(2, 3).Direction);
        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(4, 1).Direction);
        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(4, 3).Direction);

        Assert.NotNull(TrackLayout.GetTrackAt<CrossTrack>(2, 2));
        Assert.NotNull(TrackLayout.GetTrackAt<CrossTrack>(4, 2));

        Assert.True(TrackLayout.GetTrackAt<SingleTrack>(3, 2).Happy);
    }

    [Fact]
    public void TwoCrosses_DragRight()
    {
        TrackTool.Execute(2, 1, new ExecuteInfo(0, 0));
        TrackTool.Execute(2, 2, new ExecuteInfo(3, 1));
        TrackTool.Execute(2, 3, new ExecuteInfo(2, 2));

        TrackTool.Execute(4, 1, new ExecuteInfo(0, 0));
        TrackTool.Execute(4, 2, new ExecuteInfo(4, 1));
        TrackTool.Execute(4, 3, new ExecuteInfo(4, 2));

        TrackTool.Execute(5, 2, new ExecuteInfo(0, 0));
        TrackTool.Execute(4, 2, new ExecuteInfo(5, 2));
        TrackTool.Execute(3, 2, new ExecuteInfo(4, 2));
        TrackTool.Execute(2, 2, new ExecuteInfo(3, 2));
        TrackTool.Execute(1, 2, new ExecuteInfo(2, 2));

        Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(1, 2).Direction);
        Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(3, 2).Direction);
        Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(5, 2).Direction);

        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(2, 1).Direction);
        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(2, 3).Direction);
        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(4, 1).Direction);
        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(4, 3).Direction);

        Assert.NotNull(TrackLayout.GetTrackAt<CrossTrack>(2, 2));
        Assert.NotNull(TrackLayout.GetTrackAt<CrossTrack>(4, 2));

        Assert.True(TrackLayout.GetTrackAt<SingleTrack>(3, 2).Happy);
    }

    [Fact]
    public void TwoCrosses_DragDown()
    {
        TrackTool.Execute(1, 2, new ExecuteInfo(0, 0));
        TrackTool.Execute(2, 2, new ExecuteInfo(1, 2));
        TrackTool.Execute(3, 2, new ExecuteInfo(2, 2));

        TrackTool.Execute(1, 4, new ExecuteInfo(0, 0));
        TrackTool.Execute(2, 4, new ExecuteInfo(1, 4));
        TrackTool.Execute(3, 4, new ExecuteInfo(2, 4));

        TrackTool.Execute(2, 1, new ExecuteInfo(0, 0));
        TrackTool.Execute(2, 2, new ExecuteInfo(2, 1));
        TrackTool.Execute(2, 3, new ExecuteInfo(2, 2));
        TrackTool.Execute(2, 4, new ExecuteInfo(2, 3));
        TrackTool.Execute(2, 5, new ExecuteInfo(2, 4));

        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(2, 1).Direction);
        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(2, 3).Direction);
        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(2, 5).Direction);

        Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(1, 2).Direction);
        Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(3, 2).Direction);
        Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(1, 4).Direction);
        Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(3, 4).Direction);

        Assert.NotNull(TrackLayout.GetTrackAt<CrossTrack>(2, 2));
        Assert.NotNull(TrackLayout.GetTrackAt<CrossTrack>(2, 4));

        Assert.True(TrackLayout.GetTrackAt<SingleTrack>(2, 3).Happy);
    }

    [Fact]
    public void TwoCrosses_DragUp()
    {
        TrackTool.Execute(1, 2, new ExecuteInfo(0, 0));
        TrackTool.Execute(2, 2, new ExecuteInfo(1, 2));
        TrackTool.Execute(3, 2, new ExecuteInfo(2, 2));

        TrackTool.Execute(1, 4, new ExecuteInfo(0, 0));
        TrackTool.Execute(2, 4, new ExecuteInfo(1, 4));
        TrackTool.Execute(3, 4, new ExecuteInfo(2, 4));

        TrackTool.Execute(2, 5, new ExecuteInfo(0, 0));
        TrackTool.Execute(2, 4, new ExecuteInfo(2, 5));
        TrackTool.Execute(2, 3, new ExecuteInfo(2, 4));
        TrackTool.Execute(2, 2, new ExecuteInfo(2, 3));
        TrackTool.Execute(2, 1, new ExecuteInfo(2, 2));

        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(2, 1).Direction);
        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(2, 3).Direction);
        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(2, 5).Direction);

        Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(1, 2).Direction);
        Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(3, 2).Direction);
        Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(1, 4).Direction);
        Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(3, 4).Direction);

        Assert.NotNull(TrackLayout.GetTrackAt<CrossTrack>(2, 2));
        Assert.NotNull(TrackLayout.GetTrackAt<CrossTrack>(2, 4));

        Assert.True(TrackLayout.GetTrackAt<SingleTrack>(2, 3).Happy);
    }

    [Fact]
    public void Horizontal()
    {
        TrackTool.Execute(1, 1, new ExecuteInfo(0, 0));
        TrackTool.Execute(2, 1, new ExecuteInfo(1, 1));

        Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(1, 1).Direction);
        Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(2, 1).Direction);
    }

    [Fact]
    public void Vertical()
    {
        TrackTool.Execute(1, 1, new ExecuteInfo(0, 0));
        TrackTool.Execute(1, 2, new ExecuteInfo(1, 1));

        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(1, 1).Direction);
        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(1, 2).Direction);
    }

    [Fact]
    public void Two_Verticals()
    {
        TrackTool.Execute(1, 1, new ExecuteInfo(0, 0));
        TrackTool.Execute(1, 2, new ExecuteInfo(1, 1));
        TrackTool.Execute(1, 3, new ExecuteInfo(1, 2));
        TrackTool.Execute(1, 4, new ExecuteInfo(1, 3));

        TrackTool.Execute(2, 2, new ExecuteInfo(0, 0));
        TrackTool.Execute(2, 3, new ExecuteInfo(2, 2));

        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(1, 1).Direction);
        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(1, 2).Direction);
        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(1, 3).Direction);
        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(1, 4).Direction);

        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(2, 2).Direction);
        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(2, 3).Direction);
    }

    [Fact]
    public void Three_Verticals()
    {
        TrackTool.Execute(1, 1, new ExecuteInfo(0, 0));
        TrackTool.Execute(1, 2, new ExecuteInfo(1, 1));
        TrackTool.Execute(1, 3, new ExecuteInfo(1, 2));
        TrackTool.Execute(1, 4, new ExecuteInfo(1, 3));
        TrackTool.Execute(1, 5, new ExecuteInfo(1, 4));

        TrackTool.Execute(2, 2, new ExecuteInfo(0, 0));
        TrackTool.Execute(2, 3, new ExecuteInfo(2, 2));
        TrackTool.Execute(2, 4, new ExecuteInfo(2, 3));

        TrackTool.Execute(3, 3, new ExecuteInfo(0, 0));

        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(1, 1).Direction);
        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(1, 2).Direction);
        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(1, 3).Direction);
        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(1, 4).Direction);
        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(1, 5).Direction);

        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(2, 2).Direction);
        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(2, 3).Direction);
        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(2, 4).Direction);

        Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(3, 3).Direction);
    }

    [Fact]
    public void LeftUp()
    {
        TrackTool.Execute(1, 2, new ExecuteInfo(0, 0));
        TrackTool.Execute(2, 1, new ExecuteInfo(1, 2));
        TrackTool.Execute(2, 2, new ExecuteInfo(2, 1));

        Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(1, 2).Direction);
        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(2, 1).Direction);
        Assert.Equal(SingleTrackDirection.LeftUp, TrackLayout.GetTrackAt<SingleTrack>(2, 2).Direction);
    }

    [Fact]
    public void LeftUpDown()
    {
        TrackTool.Execute(1, 3, new ExecuteInfo(0, 0));
        TrackTool.Execute(2, 1, new ExecuteInfo(0, 0));
        TrackTool.Execute(2, 2, new ExecuteInfo(0, 0));
        TrackTool.Execute(2, 5, new ExecuteInfo(0, 0));
        TrackTool.Execute(2, 4, new ExecuteInfo(0, 0));
        TrackTool.Execute(2, 3, new ExecuteInfo(0, 0));

        Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(1, 3).Direction);
        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(2, 1).Direction);
        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(2, 2).Direction);
        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(2, 4).Direction);
        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(2, 5).Direction);
        Assert.Equal(TIntersectionDirection.LeftDown_LeftUp, TrackLayout.GetTrackAt<TIntersection>(2, 3).Direction);
    }

    [Fact]
    public void RightUpDown()
    {
        TrackTool.Execute(3, 3, new ExecuteInfo(0, 0));
        TrackTool.Execute(2, 1, new ExecuteInfo(0, 0));
        TrackTool.Execute(2, 2, new ExecuteInfo(0, 0));
        TrackTool.Execute(2, 5, new ExecuteInfo(0, 0));
        TrackTool.Execute(2, 4, new ExecuteInfo(0, 0));
        TrackTool.Execute(2, 3, new ExecuteInfo(0, 0));

        Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(3, 3).Direction);
        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(2, 1).Direction);
        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(2, 2).Direction);
        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(2, 4).Direction);
        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(2, 5).Direction);
        Assert.Equal(TIntersectionDirection.RightUp_RightDown, TrackLayout.GetTrackAt<TIntersection>(2, 3).Direction);
    }

    [Fact]
    public void RightUpDown_VerticalFirst()
    {
        TrackTool.Execute(2, 1, new ExecuteInfo(0, 0));
        TrackTool.Execute(2, 2, new ExecuteInfo(2, 1));
        TrackTool.Execute(2, 3, new ExecuteInfo(2, 2));

        TrackTool.Execute(3, 2, new ExecuteInfo(0, 0));
        TrackTool.Execute(2, 2, new ExecuteInfo(3, 2));

        Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(3, 2).Direction);
        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(2, 1).Direction);
        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(2, 3).Direction);
        Assert.Equal(TIntersectionDirection.RightUp_RightDown, TrackLayout.GetTrackAt<TIntersection>(2, 2).Direction);
    }

    [Fact]
    public void LeftRightUp()
    {
        TrackTool.Execute(1, 2, new ExecuteInfo(0, 0));
        TrackTool.Execute(2, 1, new ExecuteInfo(0, 0));
        TrackTool.Execute(3, 2, new ExecuteInfo(0, 0));
        TrackTool.Execute(2, 2, new ExecuteInfo(0, 0));

        Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(1, 2).Direction);
        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(2, 1).Direction);
        Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(3, 2).Direction);
        Assert.Equal(TIntersectionDirection.LeftUp_RightUp, TrackLayout.GetTrackAt<TIntersection>(2, 2).Direction);
    }

    [Fact]
    public void LeftRightDown()
    {
        TrackTool.Execute(1, 2, new ExecuteInfo(0, 0));
        TrackTool.Execute(2, 2, new ExecuteInfo(1, 2));
        TrackTool.Execute(3, 2, new ExecuteInfo(2, 2));
        TrackTool.Execute(2, 3, new ExecuteInfo(0, 0));
        TrackTool.Execute(2, 2, new ExecuteInfo(2, 3));

        Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(1, 2).Direction);
        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(2, 3).Direction);
        Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(3, 2).Direction);
        Assert.Equal(TIntersectionDirection.RightDown_LeftDown, TrackLayout.GetTrackAt<TIntersection>(2, 2).Direction);
    }

    [Fact]
    public void LeftDown()
    {
        TrackTool.Execute(1, 1, new ExecuteInfo(0, 0));
        TrackTool.Execute(2, 1, new ExecuteInfo(1, 1));
        TrackTool.Execute(2, 2, new ExecuteInfo(2, 1));

        Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(1, 1).Direction);
        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(2, 2).Direction);
        Assert.Equal(SingleTrackDirection.LeftDown, TrackLayout.GetTrackAt<SingleTrack>(2, 1).Direction);
    }

    [Fact]
    public void RightUp()
    {
        TrackTool.Execute(1, 1, new ExecuteInfo(0, 0));
        TrackTool.Execute(1, 2, new ExecuteInfo(1, 1));
        TrackTool.Execute(2, 2, new ExecuteInfo(1, 2));

        Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(2, 2).Direction);
        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(1, 1).Direction);
        Assert.Equal(SingleTrackDirection.RightUp, TrackLayout.GetTrackAt<SingleTrack>(1, 2).Direction);
    }

    [Fact]
    public void RightDown()
    {
        TrackTool.Execute(2, 1, new ExecuteInfo(0, 0));
        TrackTool.Execute(1, 1, new ExecuteInfo(2, 1));
        TrackTool.Execute(1, 2, new ExecuteInfo(1, 1));

        Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(2, 1).Direction);
        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(1, 2).Direction);
        Assert.Equal(SingleTrackDirection.RightDown, TrackLayout.GetTrackAt<SingleTrack>(1, 1).Direction);
    }

    [Fact]
    public void RightUpDown_DrawOver()
    {
        TrackTool.Execute(1, 1, new ExecuteInfo(0, 0));
        TrackTool.Execute(1, 2, new ExecuteInfo(1, 1));
        TrackTool.Execute(1, 3, new ExecuteInfo(1, 2));
        TrackTool.Execute(2, 2, new ExecuteInfo(1, 3));

        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(1, 1).Direction);
        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(1, 2).Direction);
        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(1, 3).Direction);
        Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(2, 2).Direction);

        TrackTool.Execute(1, 2, new ExecuteInfo(2, 2));

        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(1, 1).Direction);
        Assert.Equal(TIntersectionDirection.RightUp_RightDown, TrackLayout.GetTrackAt<TIntersection>(1, 2).Direction);
        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(1, 3).Direction);
        Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(2, 2).Direction);
    }

    [Fact]
    public void LeftUpDown_DrawOver()
    {
        TrackTool.Execute(3, 1, new ExecuteInfo(0, 0));
        TrackTool.Execute(3, 2, new ExecuteInfo(3, 1));
        TrackTool.Execute(3, 3, new ExecuteInfo(3, 2));
        TrackTool.Execute(2, 2, new ExecuteInfo(3, 3));

        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(3, 1).Direction);
        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(3, 2).Direction);
        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(3, 3).Direction);
        Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(2, 2).Direction);

        TrackTool.Execute(3, 2, new ExecuteInfo(2, 2));

        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(3, 1).Direction);
        Assert.Equal(TIntersectionDirection.LeftDown_LeftUp, TrackLayout.GetTrackAt<TIntersection>(3, 2).Direction);
        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(3, 3).Direction);
        Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(2, 2).Direction);
    }

    [Fact]
    public void LeftRightDown_DrawOver()
    {
        TrackTool.Execute(1, 1, new ExecuteInfo(0, 0));
        TrackTool.Execute(2, 1, new ExecuteInfo(1, 1));
        TrackTool.Execute(3, 1, new ExecuteInfo(2, 1));
        TrackTool.Execute(2, 2, new ExecuteInfo(3, 1));

        Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(1, 1).Direction);
        Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(2, 1).Direction);
        Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(3, 1).Direction);
        Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(2, 2).Direction);

        TrackTool.Execute(2, 1, new ExecuteInfo(2, 2));

        Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(1, 1).Direction);
        Assert.Equal(TIntersectionDirection.RightDown_LeftDown, TrackLayout.GetTrackAt<TIntersection>(2, 1).Direction);
        Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(3, 1).Direction);
        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(2, 2).Direction);
    }

    [Fact]
    public void LeftRightUp_DrawOver()
    {
        TrackTool.Execute(1, 3, new ExecuteInfo(0, 0));
        TrackTool.Execute(2, 3, new ExecuteInfo(1, 3));
        TrackTool.Execute(3, 3, new ExecuteInfo(2, 3));
        TrackTool.Execute(2, 2, new ExecuteInfo(3, 3));

        Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(1, 3).Direction);
        Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(2, 3).Direction);
        Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(3, 3).Direction);
        Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(2, 2).Direction);

        TrackTool.Execute(2, 3, new ExecuteInfo(2, 2));

        Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(1, 3).Direction);
        Assert.Equal(TIntersectionDirection.LeftUp_RightUp, TrackLayout.GetTrackAt<TIntersection>(2, 3).Direction);
        Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(3, 3).Direction);
        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(2, 2).Direction);
    }

    [Fact]
    public void Happiness()
    {
        TrackTool.Execute(1, 1, new ExecuteInfo(0, 0));

        Assert.False(TrackLayout.GetTrackAt<SingleTrack>(1, 1).Happy);

        TrackTool.Execute(1, 2, new ExecuteInfo(1, 1));

        Assert.False(TrackLayout.GetTrackAt<SingleTrack>(1, 1).Happy);
        Assert.False(TrackLayout.GetTrackAt<SingleTrack>(1, 2).Happy);

        TrackTool.Execute(1, 3, new ExecuteInfo(1, 2));

        Assert.False(TrackLayout.GetTrackAt<SingleTrack>(1, 1).Happy);
        Assert.True(TrackLayout.GetTrackAt<SingleTrack>(1, 2).Happy);
        Assert.False(TrackLayout.GetTrackAt<SingleTrack>(1, 3).Happy);
    }

    [Fact]
    public void Loop()
    {
        StartDrawTrack(4, 1);
        DrawTrack(DrawDirection.Right);
        DrawTrack(DrawDirection.Down);
        DrawTrack(DrawDirection.Down);
        DrawTrack(DrawDirection.Left);
        DrawTrack(DrawDirection.Left);
        DrawTrack(DrawDirection.Left);
        DrawTrack(DrawDirection.Left);
        DrawTrack(DrawDirection.Up);
        DrawTrack(DrawDirection.Up);
        DrawTrack(DrawDirection.Right);
        DrawTrack(DrawDirection.Right);

        Assert.Equal(SingleTrackDirection.RightDown, TrackLayout.GetTrackAt<SingleTrack>(1, 1).Direction);
        Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(2, 1).Direction);
        Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(3, 1).Direction);
        Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(4, 1).Direction);
        Assert.Equal(SingleTrackDirection.LeftDown, TrackLayout.GetTrackAt<SingleTrack>(5, 1).Direction);

        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(1, 2).Direction);
        Assert.Equal(SingleTrackDirection.Vertical, TrackLayout.GetTrackAt<SingleTrack>(5, 2).Direction);

        Assert.Equal(SingleTrackDirection.RightUp, TrackLayout.GetTrackAt<SingleTrack>(1, 3).Direction);
        Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(2, 3).Direction);
        Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(3, 3).Direction);
        Assert.Equal(SingleTrackDirection.Horizontal, TrackLayout.GetTrackAt<SingleTrack>(4, 3).Direction);
        Assert.Equal(SingleTrackDirection.LeftUp, TrackLayout.GetTrackAt<SingleTrack>(5, 3).Direction);
    }
}
