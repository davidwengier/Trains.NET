using Trains.NET.Engine.Tracks;

namespace Trains.NET.Engine
{
    [Order(20)]
    public class SandTool : ITool
    {
        private readonly ITerrainMap _terrainMap;

        public ToolMode Mode => ToolMode.Build;
        public string Name => "Sand";
        public string Category => "Terrain";

        public SandTool(ITerrainMap terrainMap)
        {
            _terrainMap = terrainMap;
        }

        public void Execute(int column, int row)
        {
            _terrainMap.SetTerrainType(column, row, TerrainType.Sand);
        }

        public bool IsValid(int column, int row) => true;
    }
}
