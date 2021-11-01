using System;

namespace Trains.NET.Engine.Sounds;

public interface ISoundGenerator : IDisposable
{
    bool IsRunning { get; }
    void Start();
    void Stop();
}
