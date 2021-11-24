using Trains.NET.Engine;

namespace Trains.NET.Tests;

internal class NullSerializer : IEntityCollectionSerializer
{
    public IEnumerable<IEntity> Deserialize(string lines)
    {
        yield break;
    }

    public string Serialize(IEnumerable<IEntity> tracks)
    {
        return "";
    }
}
