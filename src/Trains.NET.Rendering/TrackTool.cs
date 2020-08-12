using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    [Order(10)]
    internal class TrackTool : ITool
    {
        private readonly ILayout<Track> _entityCollection;

        public ToolMode Mode => ToolMode.Build;
        public string Name => "Track";
        public string Category => "Train";

        public TrackTool(ILayout<Track> trackLayout)
        {
            _entityCollection = trackLayout;
        }

        public void Execute(int column, int row)
        {
            _entityCollection.Add(column, row, new Track());
        }

        public bool IsValid(int column, int row) => _entityCollection.IsAvailable(column, row);
    }
}
