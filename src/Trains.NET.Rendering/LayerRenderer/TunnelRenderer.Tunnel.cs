namespace Trains.NET.Rendering
{
    public partial class TunnelRenderer
    {
        private enum Tunnel
        {
            NoTunnels = 0,
            Top = 1,
            Right = 2,
            Bottom = 4,
            Left = 8,

            TopRightCorner = Top | Right,
            BottomRightCorner = Bottom | Right,
            TopLeftCorner = Top | Left,
            BottomLeftCorner = Bottom | Left,

            StraightHorizontal = Left | Right,
            StraightVertical = Top | Bottom,

            ThreewayNoLeft = Right | Top | Bottom,
            ThreewayNoUp = Left | Bottom | Right,
            ThreewayNoRight = Left | Top | Bottom,
            ThreewayNoDown = Left | Top | Right,

            FourWayIntersection = Left | Top | Bottom | Right
        }
    }
}
