using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    [Order(10)]
    public class TrackTool : ITool
    {
        private readonly ILayout<Track> _entityCollection;
        private readonly ITerrainMap _terrainMap;

        public ToolMode Mode => ToolMode.Build;
        public string Name => "Track";
        public string Category => "Train";

        public TrackTool(ILayout<Track> trackLayout, ITerrainMap terrainMap)
        {
            _entityCollection = trackLayout;
            _terrainMap = terrainMap;
        }

        public void Execute(int column, int row)
        {
            _entityCollection.Add(column, row, new Track());
        }

        public bool IsValid(int column, int row) => _entityCollection.IsAvailable(column, row) &&
            _terrainMap.GetTerrainOrDefault(column, row).TerrainType != TerrainType.Water;
    }
}
