using Trains.NET.Engine;

namespace Trains.Emoji;

internal class NullStorage : IGameStorage
{
    public string? Read(string key)
    {
        return null;
    }

    public void Write(string key, string value)
    {
    }
}
