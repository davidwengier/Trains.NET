using Trains.NET.Engine.Terrain;

namespace Trains.NET.Engine
{
    [Order(20)]
    internal class WaterTool : ITool
    {

        private readonly ITerrainMap _terrainMap;

        public WaterTool(ITerrainMap terrainMap)
        {
            _terrainMap = terrainMap;
        }

        public ToolMode Mode => ToolMode.Build;
        public string Name => "Water";

        public string Category => "Terrain";

        public void Execute(int column, int row)
        {
            _terrainMap.SetTerrainType(column, row, TerrainType.Water);
        }

        public bool IsValid(int column, int row) => true;
    }
}
