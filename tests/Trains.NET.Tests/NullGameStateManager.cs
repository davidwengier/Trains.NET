using Trains.NET.Engine;
using Trains.NET.Engine.Utilities;

namespace Trains.NET.Tests;

internal class NullGameStateManager : IGameStateManager
{
    public SaveModes SaveMode { get; private set; }

    public void ChangeSaveMode()
    {
    }

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
