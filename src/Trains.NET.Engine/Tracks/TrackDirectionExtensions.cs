namespace Trains.NET.Engine
{
    public static class TrackDirectionExtensions
    {
        public static float TrackRotationAngle(this TrackDirection direction) => direction switch
        {
            TrackDirection.LeftUp => 0,
            TrackDirection.LeftUp_RightUp => 0,

            TrackDirection.RightUp => 90,
            TrackDirection.RightUp_RightDown => 90,

            TrackDirection.RightDown => 180,
            TrackDirection.RightDown_LeftDown => 180,

            TrackDirection.LeftDown => 270,
            TrackDirection.LeftDown_LeftUp => 270,

            _ => 0
        };
    }
}
