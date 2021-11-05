using Trains.NET.Engine;

namespace Trains.NET.Tests;

internal class NullGameStateManager : IGameStateManager
{
    public void Load(int columns, int rows)
    {
    }

    public void Reset(int columns, int rows)
    {
    }

    public void Save()
    {
    }
}
