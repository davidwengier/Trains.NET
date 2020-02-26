using Trains.NET.Engine;
using Xunit;

#nullable disable

namespace Trains.NET.Tests
{
    public class TrainTests
    {
        [Fact]
        public void Test()
        {
            Track track = new Track(null);

            Train train = new Train();
            train.Column = 1;
            train.RelativeLeft = 0.001f;
            train.RelativeTop = 0.5f;
            train.Angle = 90f;
            train.Move(1, track);
        }
    }
}
