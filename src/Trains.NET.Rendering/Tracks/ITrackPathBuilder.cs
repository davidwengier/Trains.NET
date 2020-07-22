namespace Trains.NET.Rendering
{
    public interface ITrackPathBuilder
    {
        IPath BuildHorizontalTrackPath();
        IPath BuildHorizontalPlankPath();
        IPath BuildCornerTrackPath();
        IPath BuildCornerPlankPath();
        IPath BuildCornerPlankPath(int plankCount);
    }
}
