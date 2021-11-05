using System;
using System.Collections.Generic;

namespace Trains.NET.Engine;

public interface ITerrainMap : IEnumerable<Terrain>
{
    event EventHandler CollectionChanged;

    Terrain Get(int column, int row);
    void Reset(int seed, int columns, int rows);
}
