using System.Diagnostics;

namespace Trains.NET.Instrumentation;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1001:Types that own disposable fields should be disposable", Justification = "Dispose is used to measure")]
public class ElapsedMillisecondsTimedStat : AveragedStat
{
    private readonly Stopwatch _sw;
    private readonly DisposableCallback _disposableCallback;
    public ElapsedMillisecondsTimedStat()
        : base(10) // Average over 10 samples
    {
        _sw = new Stopwatch();
        _disposableCallback = new DisposableCallback();
        _disposableCallback.Disposing += (o, e) => Stop();
    }
    public void Start() => _sw.Restart();
    public void Stop()
    {
        _sw.Stop();
        SetValue(_sw.ElapsedMilliseconds);
    }
    public IDisposable Measure()
    {
        Start();
        return _disposableCallback;
    }
    public override string GetDescription()
    {
        if (this.Value == null)
        {
            return "null";
        }
        if (this.Value < 0.01)
        {
            return "< 0.01ms";
        }
        return Math.Round(this.Value ?? 0, 2).ToString("0.00") + "ms";
    }
}
