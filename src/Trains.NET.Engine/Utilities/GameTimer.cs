using System;
using System.Diagnostics;
using System.Timers;

namespace Trains.NET.Engine
{
    // Disable GameTimer :)
    public class GameTimer// : ITimer 
    {
        private readonly Timer _timer;
        private bool _invoking = false;
        private readonly object _invokingLockObject = new object();
        private long _timeSinceLastTick;
        private long _lastTick = 0;
        private readonly Stopwatch _stopwatch = Stopwatch.StartNew();

        public long TimeSinceLastTick => _timeSinceLastTick;

        public double Interval
        {
            get => _timer.Interval;
            set => _timer.Interval = value;
        }

        public event EventHandler? Elapsed;

        public GameTimer()
        {
            _timer = new Timer();
            _timer.Elapsed += _timer_Elapsed;
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_invoking) return;
            lock (_invokingLockObject)
            {
                if (_invoking) return;
                _invoking = true;
            }
            try
            {
                long time = _stopwatch.ElapsedMilliseconds;
                _timeSinceLastTick = time - _lastTick;
                _lastTick = time;
                Elapsed?.Invoke(sender, e);
            }
            finally
            {
                lock (_invokingLockObject)
                {
                    _invoking = false;
                }
            }
        }

        public void Dispose() => _timer.Dispose();

        public void Start() => _timer.Start();

        public void Stop() => _timer.Stop();
    }
}
