using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Trains.NET.Engine;

public interface ILayout : IEnumerable<IStaticEntity>
{
    event EventHandler CollectionChanged;

    bool TryGet(int column, int row, [NotNullWhen(true)] out IStaticEntity? entity);
    bool TryGet<T>(int column, int row, [NotNullWhen(true)] out T? entity) where T : class, IStaticEntity;
    void Set(int column, int row, IStaticEntity entity);
    void Add(int column, int row, IStaticEntity entity);
    void Remove(int column, int row);
    void RaiseCollectionChanged();
}
