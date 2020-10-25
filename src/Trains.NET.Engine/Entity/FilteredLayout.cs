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

        public void Add(int column, int row, IEnumerable<IStaticEntityFactory<T>> entityFactories)
        {
            T? entity;
            if (TryGet(column, row, out T? existing))
            {
                entity = FindNextEntity(column, row, existing, entityFactories);
                if (entity is not null)
                {
                    _layout.Remove(column, row);
                }
            }
            else
            {
                entity = CreateNewStaticEntity(column, row, entityFactories);
            }

            if (entity is null)
            {
                return;
            }
            _layout.Add(column, row, entity);
        }

        public T? FindNextEntity(int column, int row, T existing, IEnumerable<IStaticEntityFactory<T>> entityFactories)
        {
            T? firstEntity = null;
            bool returnNext = false;
            foreach (var factory in entityFactories)
            {
                if (factory.TryCreateEntity(column, row, out var newEntity))
                {
                    firstEntity ??= newEntity;
                    if (returnNext)
                    {
                        return newEntity;
                    }
                    if (newEntity.GetType().Equals(existing.GetType()))
                    {
                        returnNext = true;
                    }
                }
            }
            if (returnNext)
            {
                return firstEntity;
            }
            return null;
        }

        private static T? CreateNewStaticEntity(int column, int row, IEnumerable<IStaticEntityFactory<T>> entityFactories)
        {
            foreach (var factory in entityFactories)
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
