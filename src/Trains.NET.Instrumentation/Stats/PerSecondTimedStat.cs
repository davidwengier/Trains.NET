using System.Diagnostics;

namespace Trains.NET.Instrumentation;

public class PerSecondTimedStat : AveragedStat
{
    private readonly Stopwatch _sw;
    public PerSecondTimedStat()
        : base(60) // Average over 60 samples
    {
        _sw = new Stopwatch();
    }

    public void Update()
    {
        if (_sw.IsRunning)
        {
            SetValue(1000.0 / _sw.ElapsedMilliseconds);
        }

        _sw.Restart();
    }

    public override string GetDescription()
    {
        if (Value == null)
        {
            return "null";
        }

        if (Value < 0.01)
        {
            return "< 0.01";
        }

        return Math.Round(Value ?? 0, 2).ToString();
    }
}
