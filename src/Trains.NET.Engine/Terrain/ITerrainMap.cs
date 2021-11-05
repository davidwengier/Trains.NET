using System;
using System.Collections.Generic;

namespace Trains.NET.Engine;

public interface ITerrainMap : IEnumerable<Terrain>
{
    int Seed { get; }
    event EventHandler CollectionChanged;
    Terrain Get(int column, int row);
    void Reset(int columns, int rows);
    void ResetToSeed(int seed, int columns, int rows);
}
