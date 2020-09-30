using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    [Order(10)]
    public class TrackTool : ITool
    {
        private readonly ILayout<Track> _entityCollection;

        public ToolMode Mode => ToolMode.Build;
        public string Name => "Track";

        public TrackTool(ILayout<Track> trackLayout)
        {
            _entityCollection = trackLayout;
        }

        public void Execute(int column, int row)
        {
            _entityCollection.Add(column, row);
        }

        public bool IsValid(int column, int row) => _entityCollection.IsAvailable(column, row);
    }
}
