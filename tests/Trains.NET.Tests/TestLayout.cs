using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Trains.NET.Engine;

namespace Trains.NET.Tests
{
    internal class TestLayout : ILayout
    {
        private readonly Dictionary<(int, int), IStaticEntity> _layout = new Dictionary<(int, int), IStaticEntity>();

        public event EventHandler CollectionChanged;

        public void Add(int column, int row, IStaticEntity entityToAdd)
        {
            entityToAdd.SetOwner(this);
            _layout.Add((column, row), entityToAdd);
        }

        public void Clear()
        {
            _layout.Clear();
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

        public bool TryGet(int column, int row, [NotNullWhen(true)] out IStaticEntity entity)
        {
            return _layout.TryGetValue((column, row), out entity);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _layout.Values.GetEnumerator();
        }

        public bool TryGet<T>(int column, int row, out T entity) where T : class, IStaticEntity
        {
            bool result = _layout.TryGetValue((column, row), out IStaticEntity staticEntity);
            entity = (T)staticEntity;
            return result;
        }
    }
}
