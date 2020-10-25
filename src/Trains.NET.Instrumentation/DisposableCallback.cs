using System;

namespace Trains.NET.Instrumentation
{
    public class DisposableCallback : IDisposable
    {
        public event EventHandler? Disposing;
        public void Dispose()
        {
            Disposing?.Invoke(this, EventArgs.Empty);
        }
    }
}
