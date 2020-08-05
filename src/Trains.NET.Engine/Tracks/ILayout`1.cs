using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Trains.NET.Engine
{
    public interface ILayout<T> : IEnumerable<T>
        where T : class, IStaticEntity
    {
        event EventHandler CollectionChanged;

        bool TryGet(int column, int row, [NotNullWhen(true)] out T? entity);
        void Clear();
        void Add(int column, int row, T track);
        bool IsAvailable(int column, int row);
    }
}
