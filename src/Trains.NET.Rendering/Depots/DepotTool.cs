using Trains.NET.Engine;

namespace Trains.NET.Rendering;

[Order(20)]
public class DepotTool(
    ILayout<Depot> depotLayout,
    ITerrainMap terrainMap,
    IEnumerable<IStaticEntityFactory<Depot>> entityFactories) : ITool
{
    private readonly ILayout<Depot> _depotLayout = depotLayout;
    private readonly ITerrainMap _terrainMap = terrainMap;
    private readonly IEnumerable<IStaticEntityFactory<Depot>> _entityFactories = entityFactories;

    public bool PauseGameDuringDrag => false;
    public string Name => "Depot";

    public void Execute(int column, int row, ExecuteInfo info)
    {
        if (info.FromColumn == 0 && _depotLayout.TryGet(column, row, out var depot))
        {
            _depotLayout.SelectedEntity = depot;
        }
        else
        {
            _depotLayout.Add(column, row, _entityFactories, info.FromColumn, info.FromRow);
            _depotLayout.SelectedEntity = null;
        }
    }

    public bool IsValid(int column, int row)
        => _depotLayout.IsAvailable(column, row) &&
            _terrainMap.Get(column, row).IsLand;
}
