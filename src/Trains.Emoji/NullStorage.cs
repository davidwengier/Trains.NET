using Trains.NET.Engine;

namespace Trains.Emoji;

internal class NullStorage : IGameStorage
{
    public string? ReadEntities()
    {
        return null;
    }

    public void WriteEntities(string entities)
    {
    }
}
