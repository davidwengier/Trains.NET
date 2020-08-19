
namespace Trains.NET.Engine
{
    [Order(20)]
    public class WaterTool : ITool
    {
        private readonly ITerrainMap _terrainMap;
        private readonly ILayout _layout;

        public ToolMode Mode => ToolMode.Build;
        public string Name => "Water";
        public string Category => "Terrain";

        public WaterTool(ITerrainMap terrainMap, ILayout layout)
        {
            _terrainMap = terrainMap;
            _layout = layout;
        }

        public void Execute(int column, int row)
        {
            _terrainMap.SetTerrainType(column, row, TerrainType.Water);
        }

        public bool IsValid(int column, int row) => !_layout.TryGet(column, row, out _);
    }
}
