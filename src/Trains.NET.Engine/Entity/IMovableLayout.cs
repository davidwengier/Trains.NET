using System.Collections.Generic;
using System.Collections.Immutable;

namespace Trains.NET.Engine;

public interface IMovableLayout
{
    void Set(IEnumerable<IMovable> trains);
    ImmutableList<IMovable> Get();
    IEnumerable<T> Get<T>() where T : IMovable;
    IMovable? GetAt(int column, int row);
    void Add(IMovable movable);
    void Remove(IMovable movable);
    void Clear();
    // TODO: Move this out of here!
    IEnumerable<(Track, Train, float)> LastTrackLeases { get; }
}
