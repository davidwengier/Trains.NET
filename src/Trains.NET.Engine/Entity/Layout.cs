using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Trains.NET.Engine;

public class Layout : ILayout, IInitializeAsync, IGameState, IGameStep
{
    public event EventHandler? CollectionChanged;

    private readonly object _gate = new object();
    private readonly IGameSerializer _gameSerializer;
    private IStaticEntity?[][] _entities = null!;
    private int _rows;

    public Layout(IGameSerializer gameSerializer)
    {
        _gameSerializer = gameSerializer;
    }

    public Task InitializeAsync(int columns, int rows)
    {
        _entities = new IStaticEntity[columns][];
        _rows = rows;
        ResetArrays();

        return Task.CompletedTask;
    }

    public void Set(int column, int row, IStaticEntity entity)
    {
        if (IsInvalid(column, row))
        {
            return;
        }

        _entities[column][row] = null;

        StoreEntity(column, row, entity);
        entity.Replaced();

        CollectionChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Add(int column, int row, IStaticEntity entity)
    {
        if (IsInvalid(column, row))
        {
            return;
        }

        if (_entities[column][row] is not null)
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
        if (IsInvalid(column, row))
        {
            return;
        }

        entity.Stored(this);
        entity.Column = column;
        entity.Row = row;
        _entities[column][row] = entity;
    }

    public void Remove(int column, int row)
    {
        if (IsInvalid(column, row))
        {
            return;
        }

        lock (_gate)
        {
            if (_entities[column][row] is { } track)
            {
                _entities[column][row] = null;
                track.Removed();
            }
        }

        CollectionChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Set(IEnumerable<IStaticEntity> tracks)
    {
        ResetArrays();

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
        if (IsInvalid(column, row))
        {
            track = null;
            return false;
        }

        track = _entities[column][row];
        return track != null;
    }

    private static bool IsInvalid(int column, int row)
    {
        return column < 0 || row < 0 || column > 200 || row > 200;
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
        ResetArrays();

        CollectionChanged?.Invoke(this, EventArgs.Empty);
    }

    private void ResetArrays()
    {
        lock (_gate)
        {
            for (int i = 0; i < _entities.Length; i++)
            {
                _entities[i] = new IStaticEntity?[_rows];
            }
        }
    }

    public IEnumerator<IStaticEntity> GetEnumerator()
    {
        if (_entities == null) yield break;
        for (int i = 0; i < _entities.Length; i++)
        {
            for (int j = 0; j < _rows; j++)
            {
                var track = _entities[i][j];
                if (track is not null)
                {
                    yield return track;
                }
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public bool Load(IGameStorage storage)
    {
        var entitiesString = storage.Read(nameof(ILayout));
        if (entitiesString is null)
            return false;

        var entities = _gameSerializer.Deserialize(entitiesString);

        var staticEntites = entities.OfType<IStaticEntity>();

        if (staticEntites is null)
            return false;

        Set(staticEntites);

        return true;
    }

    public void Save(IGameStorage storage)
    {
        var entities = _gameSerializer.Serialize(this);
        storage.Write(nameof(ILayout), entities);
    }

    void IGameState.Reset()
        => Clear();

    public void Update(long timeSinceLastTick)
    {
        foreach (IUpdatableEntity entity in this.OfType<IUpdatableEntity>())
        {
            entity.Update();
        }
    }
}
