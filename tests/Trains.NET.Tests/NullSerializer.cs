using System.Collections.Generic;
using Trains.NET.Engine;

namespace Trains.NET.Tests;

internal class NullSerializer : IGameSerializer
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
