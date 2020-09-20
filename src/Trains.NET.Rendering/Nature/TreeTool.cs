using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    [Order(15)]
    public class TreeTool : ITool
    {
        private readonly ILayout<Tree> _entityCollection;
        private readonly ITerrainMap _terrainMap;

        public ToolMode Mode => ToolMode.Build;
        public string Name => "Tree";


        public TreeTool(ILayout<Tree> trackLayout, ITerrainMap terrainMap)
        {
            _entityCollection = trackLayout;
            _terrainMap = terrainMap;
        }

        public void Execute(int column, int row)
        {
            _entityCollection.Add(column, row, new Tree());
        }

        public bool IsValid(int column, int row) => _entityCollection.IsAvailable(column, row) &&
            _terrainMap.Get(column, row).IsLand;
    }
}
