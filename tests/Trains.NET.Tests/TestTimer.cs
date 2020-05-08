using System;
using Trains.NET.Engine;

namespace Trains.NET.Tests
{
    public class TestTimer : ITimer
    {
        public double Interval { get; set; }

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
}
