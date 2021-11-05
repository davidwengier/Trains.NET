using System;

namespace Trains.NET.Engine;

public interface IGameBoard : IDisposable
{
    bool Enabled { get; set; }

    void ClearAll();
}
