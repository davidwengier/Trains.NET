using System;
using System.Timers;

namespace Trains.NET.Engine
{
    internal class GameTimer : ITimer
    {
        private readonly Timer _timer;
        private bool _invoking = false;
        private readonly object _invokingLockObject = new object();

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
