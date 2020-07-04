using Trains.NET.Engine.Tracks;

namespace Trains.NET.Engine
{
    [Order(20)]
    internal class TerrainTool : ITool
    {

        private readonly ITerrainMap _terrainLayout;

        public TerrainTool(ITerrainMap terrainLayout)
        {
            _terrainLayout = terrainLayout;
        }

        public string Name => "Terrain";

        public void Execute(int column, int row)
        {
            _terrainLayout.AddTerrain(new Terrain { Column = column, Row = row, Altitude = 55 });
        }

        public bool IsValid(int column, int row) => true;
    }
}
