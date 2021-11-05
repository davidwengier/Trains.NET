using System;
using System.Collections;
using System.Collections.Generic;
using Trains.NET.Engine;

namespace Trains.NET.Tests;

internal class FlatTerrainMap : ITerrainMap
{
    public event EventHandler CollectionChanged;

    public Terrain Get(int column, int row)
    {
        return new Terrain() { Column = column, Row = row, Height = Terrain.FirstLandHeight };
    }

    public void Reset(int seed, int columns, int rows)
    {
        CollectionChanged?.Invoke(this, EventArgs.Empty);
    }

    public IEnumerator<Terrain> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }
}
