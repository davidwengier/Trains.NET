namespace Trains.NET.Instrumentation;

public abstract class AveragedStat(int sampleCount) : IStat
{
    public double? Value { get; private set; }
    private readonly int _sampleCount = sampleCount;

    protected void SetValue(double value)
    {
        if (this.Value == null)
        {
            this.Value = value;
        }
        else
        {
            this.Value = (this.Value * (_sampleCount - 1) + value) / _sampleCount;
        }
    }
    public abstract string GetDescription();

    public bool ShouldShow() => this.Value.HasValue;
}
