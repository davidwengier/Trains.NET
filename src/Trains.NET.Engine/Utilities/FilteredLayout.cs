using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Trains.NET.Engine
{
    public class FilteredLayout<T> : ILayout<T>
        where T : class, IStaticEntity
    {
        private readonly ILayout _layout;

        public event EventHandler? CollectionChanged;

        public FilteredLayout(ILayout layout)
        {
            _layout = layout;
            _layout.CollectionChanged += (s, e) => CollectionChanged?.Invoke(s, e);
        }

        public bool TryGet(int column, int row, [NotNullWhen(true)] out T? entity)
        {
            _layout.TryGet(column, row, out entity);
            return entity != null;
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (T entity in _layout.OfType<T>())
            {
                yield return entity;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Clear()
        {
            _layout.Clear();
        }

        public void Add(int column, int row, T track)
        {
            _layout.Add(column, row, track);
        }

        public bool IsAvailable(int column, int row)
        {
            _layout.TryGet(column, row, out IStaticEntity? entity);
            return entity == null || entity is T;
        }
    }
}
