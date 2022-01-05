using Silk.NET.Maths;
using Silk.NET.Windowing;

namespace SilkTrains;

internal static class Hacks
{
    // TODO: Replace this with the real IView.PointToFramebuffer when 2.12 is released without the divide by zero bug :O
    public static Vector2D<int> NotBuggedPointToFramebuffer(this IView view, Vector2D<int> point)
        => point * (view.FramebufferSize / view.Size);
}
