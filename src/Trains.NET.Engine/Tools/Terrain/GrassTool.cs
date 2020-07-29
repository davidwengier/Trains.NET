using Trains.NET.Engine.Terrain;

namespace Trains.NET.Engine
{
    [Order(20)]
    internal class GrassTool : ITool
    {

        private readonly ITerrainMap _terrainMap;

        public GrassTool(ITerrainMap terrainMap)
        {
            _terrainMap = terrainMap;
        }

        public ToolMode Mode => ToolMode.Build;
        public string Name => "Grass";

        public string Category => "Terrain";

        public void Execute(int column, int row)
        {
            _terrainMap.SetTerrainType(column, row, TerrainType.Grass);
        }

        public bool IsValid(int column, int row) => true;
    }
}
