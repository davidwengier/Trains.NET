using System.Collections.Generic;
using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    [Order(15)]
    public class TreeTool : ITool
    {
        private readonly ILayout<Tree> _entityCollection;
        private readonly ITerrainMap _terrainMap;
        private readonly IEnumerable<IStaticEntityFactory<Tree>> _entityFactories;

        public ToolMode Mode => ToolMode.Build;
        public string Name => "Tree";

        public TreeTool(ILayout<Tree> trackLayout, ITerrainMap terrainMap, IEnumerable<IStaticEntityFactory<Tree>> entityFactories)
        {
            _entityCollection = trackLayout;
            _terrainMap = terrainMap;
            _entityFactories = entityFactories;
        }

        public void Execute(int column, int row, bool isPartOfDrag)
        {
            _entityCollection.Add(column, row, _entityFactories, isPartOfDrag, 0, 0);
        }

        public bool IsValid(int column, int row) => _entityCollection.IsAvailable(column, row) &&
            _terrainMap.Get(column, row).IsLand;
    }
}
