using System;
using System.Diagnostics;
using System.Threading;

namespace Trains.NET.Engine
{
    public class GameThreadTimer : ITimer
    {
        private const int MinimumSleepTime = 1;

        public double Interval { get; set; }
        public long TimeSinceLastTick { get; private set; }

        public event EventHandler? Elapsed;

        private bool _elapsedEventEnabled = false;
        private long _lastTick = 0;
        private int _sleepTime = 16;
        private bool _threadLoopEnabled = true;

        private readonly Thread _gameThread;
        private readonly Stopwatch _stopwatch = Stopwatch.StartNew();

        public GameThreadTimer()
        {
            _gameThread = new Thread(ThreadLoop);
            _gameThread.Start();
        }

        private void ThreadLoop()
        {
            while(_threadLoopEnabled)
            {
                Thread.Sleep(_sleepTime);
                if (_elapsedEventEnabled)
                {
                    long time = _stopwatch.ElapsedMilliseconds;
                    this.TimeSinceLastTick = time - _lastTick;
                    _lastTick = time;
                    Elapsed?.Invoke(null, null);
                    

                   _sleepTime = Math.Max(MinimumSleepTime, (int)this.Interval - (int)(_stopwatch.ElapsedMilliseconds - time));
                }
                else
                {
                    _sleepTime = Math.Max(MinimumSleepTime, (int)this.Interval);
                }
            }
        }

        public void Dispose()
        {
            _threadLoopEnabled = false;
            _gameThread.Join();
        }

        public void Start() => _elapsedEventEnabled = true;

        public void Stop() => _elapsedEventEnabled = false;
    }
}
