using System.Collections.Generic;
using System.Collections.Immutable;

namespace Trains.NET.Engine;

public class MovableLayout : IMovableLayout
{
    private ImmutableList<IMovable> _movables = ImmutableList<IMovable>.Empty;
    public MovableLayout()
    {

    }

    public void Set(IEnumerable<IMovable> movables)
        => _movables = ImmutableList.CreateRange(movables);

    public ImmutableList<IMovable> Get()
        => _movables;

    public void Add(IMovable movable)
        => _movables = _movables.Add(movable);

    public void Remove(IMovable movable)
        => _movables = _movables.Remove(movable);

    public void Clear()
        => _movables = _movables.Clear();
}
