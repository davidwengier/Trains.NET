using Trains.NET.Engine;

namespace Trains.NET.Tests;

internal class NullGameStateManager : IGameStateManager
{
    public bool AutosaveEnabled { get; set; }

    public void Load()
    {
    }

    public void Reset()
    {
    }

    public void Save()
    {
    }
}
