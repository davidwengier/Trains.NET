using Trains.NET.Engine.Tracks;

namespace Trains.NET.Engine
{
    [Order(20)]
    internal class TerrainTool : ITool
    {

        private readonly ITerrainMap _terrainMap;

        public TerrainTool(ITerrainMap terrainMap)
        {
            _terrainMap = terrainMap;
        }

        public ToolMode Mode => ToolMode.Build;
        public string Name => "Terrain";

        public void Execute(int column, int row)
        {
            _terrainMap.AddTerrain(new Terrain { Column = column, Row = row, Altitude = 55 });
        }

        public bool IsValid(int column, int row) => true;
    }
}
