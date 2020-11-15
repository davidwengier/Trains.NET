using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Trains.NET.Engine
{
    public class Layout : ILayout
    {
        public event EventHandler? CollectionChanged;

        private ImmutableDictionary<(int, int), IStaticEntity> _entities = ImmutableDictionary<(int, int), IStaticEntity>.Empty;

        public void Set(int column, int row, IStaticEntity entity)
        {
            if (_entities.TryGetValue((column, row), out IStaticEntity? track))
            {
                _entities = _entities.Remove((column, row));
            }
            StoreEntity(column, row, entity);
            entity.Replaced();

            CollectionChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Add(int column, int row, IStaticEntity entity)
        {
            if (_entities.TryGetValue((column, row), out IStaticEntity? track))
            {
                Set(column, row, entity);
            }
            else
            {
                StoreEntity(column, row, entity);
                entity.Created();

                CollectionChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void StoreEntity(int column, int row, IStaticEntity entity)
        {
            entity.Stored(this);
            entity.Column = column;
            entity.Row = row;
            _entities = _entities.Add((column, row), entity);
        }

        public void Remove(int column, int row)
        {
            if (_entities.TryGetValue((column, row), out IStaticEntity? track))
            {
                _entities = _entities.Remove((column, row));
                track.Removed();
            }

            CollectionChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Set(IEnumerable<IStaticEntity> tracks)
        {
            _entities = _entities.Clear();

            foreach (IStaticEntity track in tracks)
            {
                StoreEntity(track.Column, track.Row, track);
            }

            CollectionChanged?.Invoke(this, EventArgs.Empty);
        }

        public void RaiseCollectionChanged()
        {
            CollectionChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool TryGet(int column, int row, [NotNullWhen(true)] out IStaticEntity? track)
        {
            return _entities.TryGetValue((column, row), out track);
        }

        public bool TryGet<T>(int column, int row, [NotNullWhen(true)] out T? entity)
            where T : class, IStaticEntity
        {
            TryGet(column, row, out IStaticEntity? staticEntity);
            entity = staticEntity as T;
            return entity != null;
        }

        public bool IsEmptyOrT<T>(int column, int row)
            where T : class, IStaticEntity
        {
            TryGet(column, row, out IStaticEntity? staticEntity);
            return staticEntity == null || staticEntity is T;
        }

        public void Clear()
        {
            _entities = _entities.Clear();

            CollectionChanged?.Invoke(this, EventArgs.Empty);
        }

        public IEnumerator<IStaticEntity> GetEnumerator()
        {
            foreach ((_, _, IStaticEntity track) in _entities)
            {
                yield return track;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
