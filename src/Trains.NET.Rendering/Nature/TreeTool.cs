using Trains.NET.Engine;

namespace Trains.NET.Rendering;

[Order(15)]
public class TreeTool(
    ILayout<Tree> trackLayout,
    ITerrainMap terrainMap,
    IEnumerable<IStaticEntityFactory<Tree>> entityFactories) : ITool
{
    private readonly ILayout<Tree> _entityCollection = trackLayout;
    private readonly ITerrainMap _terrainMap = terrainMap;
    private readonly IEnumerable<IStaticEntityFactory<Tree>> _entityFactories = entityFactories;

    public ToolMode Mode => ToolMode.Build;
    public string Name => "Tree";

    public void Execute(int column, int row, ExecuteInfo info)
    {
        _entityCollection.Add(column, row, _entityFactories, info.FromColumn, info.FromRow);
    }

    public bool IsValid(int column, int row) => _entityCollection.IsAvailable(column, row) &&
        _terrainMap.Get(column, row).IsLand;
}
