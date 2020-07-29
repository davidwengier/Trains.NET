using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Trains.NET.Engine
{
    public interface IStaticEntityCollection : IEnumerable<IStaticEntity>
    {
        event EventHandler CollectionChanged;

        bool IsEmptyOrT<T>(int column, int row) where T : class, IStaticEntity;
        bool TryGet(int column, int row, [NotNullWhen(true)] out IStaticEntity? entity);
        bool TryGet<T>(int column, int row, [NotNullWhen(true)] out T? entity) where T : class, IStaticEntity;
        void Set(IEnumerable<IStaticEntity> entities);
        void Clear();
        void Add(int column, int row, IStaticEntity entityToAdd);
        void Remove(int column, int row);
        void RaiseCollectionChanged();
    }
}
