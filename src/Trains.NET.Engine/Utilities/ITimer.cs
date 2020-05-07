using System;

namespace Trains.NET.Engine
{
    public interface ITimer : IDisposable
    {
        double Interval { get; set; }

        event EventHandler Elapsed;

        void Start();
        void Stop();
    }
}
