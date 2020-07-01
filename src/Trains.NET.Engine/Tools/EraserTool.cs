using Trains.NET.Engine.Tracks;

namespace Trains.NET.Engine
{
    [Order(20)]
    internal class EraserTool : ITool
    {
        private readonly ITrackLayout _trackLayout;

        public string Name => "Eraser";

        public EraserTool(ITrackLayout trackLayout)
        {
            _trackLayout = trackLayout;
        }

        public void Execute(int column, int row)
        {
            _trackLayout.RemoveTrack(column, row);
        }

        public bool IsValid(int column, int row) => _trackLayout.TryGet(column, row, out _);
    }
}
