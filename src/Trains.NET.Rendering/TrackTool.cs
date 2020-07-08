using Trains.NET.Engine.Tracks;
using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    [Order(10)]
    internal class TrackTool : ITool, IPreviewableTool
    {
        private readonly ITrackLayout _trackLayout;
        private readonly ITrackRenderer _trackRenderer;

        public string Name => "Track";

        public TrackTool(ITrackLayout trackLayout, ITrackRenderer trackRenderer)
        {
            _trackLayout = trackLayout;
            _trackRenderer = trackRenderer;
        }

        public void Execute(int column, int row)
        {
            _trackLayout.AddTrack(column, row);
        }

        public bool IsValid(int column, int row) => true;

        public void RenderPreview(ICanvas canvas, int column, int row)
        {
            var track = new Track(_trackLayout)
            {
                Column = column,
                Row = row
            };
            track.Direction = track.GetBestTrackDirection(true);
            _trackRenderer.Render(canvas, track);
        }
    }
}
