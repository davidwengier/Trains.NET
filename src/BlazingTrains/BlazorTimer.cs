using System.Diagnostics;
using Trains.NET.Engine;

namespace BlazingTrains
{
    public class BlazorTimer : ITimer
    {
        private CancellationTokenSource? _cts;
        private readonly Stopwatch _stopwatch = Stopwatch.StartNew();
        private long _lastTick;

        public double Interval { get; set; }

        public long TimeSinceLastTick { get; private set; }

        public event EventHandler? Elapsed;

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            Stop();
            _cts = new CancellationTokenSource();
            _ = StartTimer();
        }

        private async Task StartTimer()
        {
            var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(this.Interval));
            while (await timer.WaitForNextTickAsync(_cts!.Token))
            {
                long time = _stopwatch.ElapsedMilliseconds;
                this.TimeSinceLastTick = time - _lastTick;
                _lastTick = time;
                Elapsed?.Invoke(this, EventArgs.Empty);
            }
        }

        public void Stop()
        {
            _cts?.Cancel();
        }
    }
}
