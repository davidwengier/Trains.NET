using Trains.NET.Engine.Tracks;
using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    [Order(10)]
    internal class TrackTool : ITool
    {
        private readonly ITrackLayout _trackLayout;

        public string Name => "Track";

        public TrackTool(ITrackLayout trackLayout)
        {
            _trackLayout = trackLayout;
        }

        public void Execute(int column, int row)
        {
            _trackLayout.AddTrack(column, row);
        }

        public bool IsValid(int column, int row) => true;
    }
}
