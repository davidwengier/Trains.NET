using System.Collections;
using Trains.NET.Engine;

namespace Trains.NET.Tests;

internal class FlatTerrainMap : ITerrainMap
{
    public event EventHandler CollectionChanged;

    public Terrain Get(int column, int row)
    {
        return new Terrain() { Column = column, Row = row, Height = Terrain.FirstLandHeight };
    }

    public IEnumerator<Terrain> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    public void Reset(int? seed)
    {
        CollectionChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Set(IEnumerable<Terrain> terrainList)
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }
}
