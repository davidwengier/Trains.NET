using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Trains.NET.Engine;

namespace Trains.NET.Tests;

internal class TestLayout : ILayout
{
    private readonly Dictionary<(int, int), IStaticEntity> _layout = new();

    public event EventHandler? CollectionChanged;

    public void Add(int column, int row, IStaticEntity entityToAdd)
    {
        entityToAdd.Stored(this);
        _layout.Add((column, row), entityToAdd);
    }

    public IEnumerator<IStaticEntity> GetEnumerator()
    {
        return _layout.Values.GetEnumerator();
    }

    public void RaiseCollectionChanged()
    {
        CollectionChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Remove(int column, int row)
    {
        _layout.Remove((column, row));
    }

    public void Set(IEnumerable<IStaticEntity> entities)
    {
        throw new NotImplementedException();
    }

    public bool TryGet(int column, int row, [NotNullWhen(true)] out IStaticEntity? entity)
    {
        return _layout.TryGetValue((column, row), out entity);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _layout.Values.GetEnumerator();
    }

    public bool TryGet<T>(int column, int row, [NotNullWhen(true)] out T? entity) where T : class, IStaticEntity
    {
        TryGet(column, row, out var staticEntity);
        entity = staticEntity as T;
        return entity != null;
    }

    public void Set(int column, int row, IStaticEntity entity)
    {
        _layout[(column, row)] = entity;
    }
}
