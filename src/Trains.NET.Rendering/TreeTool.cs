using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    [Order(15)]
    public class TreeTool : ITool
    {
        private readonly ILayout<Tree> _entityCollection;

        public ToolMode Mode => ToolMode.Build;
        public string Name => "Tree";

        public string Category => "Nature";

        public TreeTool(ILayout<Tree> trackLayout)
        {
            _entityCollection = trackLayout;
        }

        public void Execute(int column, int row)
        {
            _entityCollection.Add(column, row, new Tree());
        }

        public bool IsValid(int column, int row) => _entityCollection.IsAvailable(column, row);
    }
}
