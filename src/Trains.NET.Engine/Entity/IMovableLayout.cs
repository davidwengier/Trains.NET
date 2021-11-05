using System.Collections.Generic;
using System.Collections.Immutable;

namespace Trains.NET.Engine;

public interface IMovableLayout
{
    void Set(IEnumerable<IMovable> trains);
    ImmutableList<IMovable> Get();
    void Add(IMovable movable);
    void Remove(IMovable movable);
    void Clear();
}
