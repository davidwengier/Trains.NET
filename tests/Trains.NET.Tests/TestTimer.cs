namespace Trains.NET.Tests;

public class TestTimer : Engine.ITimer
{
    public double Interval { get; set; }

    public long TimeSinceLastTick => 16;

    public event EventHandler Elapsed;

    public void Dispose()
    {
    }

    public void Start()
    {
    }

    public void Stop()
    {
    }

    public void Tick()
    {
        Elapsed?.Invoke(this, EventArgs.Empty);
    }
}
