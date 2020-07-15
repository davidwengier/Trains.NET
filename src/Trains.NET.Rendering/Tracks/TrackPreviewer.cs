using Trains.NET.Engine;
using Trains.NET.Engine.Tracks;

namespace Trains.NET.Rendering.Tracks
{
    [Context(typeof(TrackTool))]
    internal class TrackPreviewer : IToolPreviewer
    {
        private readonly ITrackLayout _trackLayout;
        private readonly ITrackRenderer _trackRenderer;

        public TrackPreviewer(ITrackLayout trackLayout, ITrackRenderer trackRenderer)
        {
            _trackLayout = trackLayout;
            _trackRenderer = trackRenderer;
        }

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
