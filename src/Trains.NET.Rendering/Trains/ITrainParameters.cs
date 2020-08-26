namespace Trains.NET.Rendering
{
    public interface ITrainParameters
    {
        float RearHeight { get; }
        float RearWidth { get; }
        float HeadWidth { get; }
        float HeadHeight { get; }
        float StrokeWidth { get; }
        float SmokeStackRadius { get; }
        float SmokeStackOffset { get; }
    }
}
