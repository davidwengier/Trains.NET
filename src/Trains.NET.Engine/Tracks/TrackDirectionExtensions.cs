namespace Trains.NET.Engine
{
    public static class TrackDirectionExtensions
    {
        public static float TrackRotationAngle(this TrackDirection direction) => direction switch
        {
            TrackDirection.LeftUp => 0,
            TrackDirection.LeftRightUp => 0,

            TrackDirection.RightUp => 90,
            TrackDirection.RightUpDown => 90,

            TrackDirection.RightDown => 180,
            TrackDirection.LeftRightDown => 180,

            TrackDirection.LeftDown => 270,
            TrackDirection.LeftUpDown => 270,

            _ => 0
        };
    }
}
