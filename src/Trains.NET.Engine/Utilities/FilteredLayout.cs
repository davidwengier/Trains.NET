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
        private readonly IEnumerable<IStaticEntityFactory<T>> _entityFactories;

        public event EventHandler? CollectionChanged;

        public FilteredLayout(ILayout layout, IEnumerable<IStaticEntityFactory<T>> entityFactories)
        {
            _layout = layout;
            _entityFactories = entityFactories;
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

        public void Add(int column, int row)
        {
            var entity = CreateNewStaticEntity(column, row);
            if (entity is null)
            {
                return;
            }

            _layout.Add(column, row, entity);
        }

        private IStaticEntity? CreateNewStaticEntity(int column, int row)
        {
            foreach (var factory in _entityFactories)
            {
                if (factory.TryCreateEntity(column, row, out var entity))
                {
                    return entity;
                }
            }
            return null;
        }

        public bool IsAvailable(int column, int row)
        {
            _layout.TryGet(column, row, out IStaticEntity? entity);
            return entity == null || entity is T;
        }
    }
}
