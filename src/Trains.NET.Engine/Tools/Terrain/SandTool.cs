
namespace Trains.NET.Engine
{
    [Order(20)]
    public class SandTool : ITool
    {
        private readonly ITerrainMap _terrainMap;
        private readonly ILayout<Tree> _treeLayout;

        public ToolMode Mode => ToolMode.Build;
        public string Name => "Sand";

        public SandTool(ITerrainMap terrainMap, ILayout<Tree> treeLayout)
        {
            _terrainMap = terrainMap;
            _treeLayout = treeLayout;
        }

        public void Execute(int column, int row)
        {
            _terrainMap.SetTerrainType(column, row, TerrainType.Sand);
        }

        public bool IsValid(int column, int row) => !_treeLayout.TryGet(column, row, out _);
    }
}
