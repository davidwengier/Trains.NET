using System;
using System.Timers;

namespace Trains.NET.Engine
{
    internal class GameTimer : ITimer
    {
        private readonly Timer _timer;

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
            Elapsed?.Invoke(sender, e);
        }

        public void Dispose() => _timer.Dispose();

        public void Start() => _timer.Start();

        public void Stop() => _timer.Stop();
    }
}
