namespace Trains.NET.Rendering;

public class TrainParameters : ITrainParameters
{
    public float RearHeight => 55.0f;

    public float RearWidth => 25.0f;

    public float HeadWidth => 62.5f;

    public float HeadHeight => 40.0f;

    public float StrokeWidth => 5.0f;

    public float SmokeStackRadius => 5.0f;

    public float SmokeStackOffset => 12 / 5f;
}
