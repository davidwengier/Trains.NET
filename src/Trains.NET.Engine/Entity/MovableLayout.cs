using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Trains.NET.Engine;

public class MovableLayout : IMovableLayout, IGameState
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

    public bool Load(IEnumerable<IEntity> entities, int columns, int rows)
    {
        var movables = entities.OfType<IMovable>();

        if (movables == null) return false;

        Set(movables);
        return true;
    }

    public IEnumerable<IEntity> Save() => _movables;

    public void Reset(int columns, int rows) => Clear();
}
