using Trains.NET.Engine.Tracks;

namespace Trains.NET.Engine
{
    [Order(20)]
    internal class HeightTool : ITool
    {
        private readonly ITerrainMap _terrainMap;

        public ToolMode Mode => ToolMode.Build;
        public string Name => "Height";
        public string Category => "Terrain";

        public HeightTool(ITerrainMap terrainMap)
        {
            _terrainMap = terrainMap;
        }

        public void Execute(int column, int row)
        {
            _terrainMap.SetTerrainHeight(column, row, height: 55);
        }

        public bool IsValid(int column, int row) => true;
    }
}
