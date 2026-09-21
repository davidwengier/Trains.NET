using System.Diagnostics.CodeAnalysis;

namespace Trains.NET.Engine;

public class DepotFactory(
    ILayout layout,
    ITerrainMap terrainMap) : IStaticEntityFactory<Depot>
{
    private readonly Random _random = new();
    private readonly ILayout _layout = layout;
    private readonly ITerrainMap _terrainMap = terrainMap;

    public IEnumerable<Depot> GetPossibleReplacements(int column, int row, Depot depot)
    {
        yield return depot.WithDirection(DepotDirection.Right);
        yield return depot.WithDirection(DepotDirection.Up);
        yield return depot.WithDirection(DepotDirection.Left);
        yield return depot.WithDirection(DepotDirection.Down);
    }

    public bool TryCreateEntity(
        int column,
        int row,
        int fromColumn,
        int fromRow,
        [NotNullWhen(true)] out Depot? entity)
    {
        entity = null;

        if (!CanCreate(column, row))
        {
            return false;
        }

        entity = Depot.CreateNew(_random.Next(), GetInitialDirection(column, row));
        return true;
    }

    public bool CanCreate(int column, int row)
        => _terrainMap.Get(column, row).IsLand &&
            !_layout.TryGet(column, row, out _);

    private DepotDirection GetInitialDirection(int column, int row)
    {
        if (_layout.TryGet(column + 1, row, out Track? right) && right.IsConnectedLeft())
        {
            return DepotDirection.Right;
        }

        if (_layout.TryGet(column, row + 1, out Track? down) && down.IsConnectedUp())
        {
            return DepotDirection.Down;
        }

        if (_layout.TryGet(column - 1, row, out Track? left) && left.IsConnectedRight())
        {
            return DepotDirection.Left;
        }

        if (_layout.TryGet(column, row - 1, out Track? up) && up.IsConnectedDown())
        {
            return DepotDirection.Up;
        }

        return DepotDirection.Right;
    }
}
