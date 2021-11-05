using Trains.NET.Engine;

namespace Trains.NET.Tests;

internal class NullStorage : IGameStorage
{
    public string ReadEntities()
    {
        return null;
    }

    public void WriteEntities(string entities)
    {
    }
}
