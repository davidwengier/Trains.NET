using Trains.NET.Engine;

namespace Trains.NET.Rendering.Tracks
{
    [Context(typeof(TrackTool))]
    public class TrackPreviewer : IToolPreviewer
    {
        private readonly ILayout _trackLayout;
        private readonly IRenderer<Track> _trackRenderer;

        public TrackPreviewer(ILayout trackLayout, IRenderer<Track> trackRenderer)
        {
            _trackLayout = trackLayout;
            _trackRenderer = trackRenderer;
        }

        public void RenderPreview(ICanvas canvas, int column, int row)
        {
            var track = new Track()
            {
                Column = column,
                Row = row
            };
            track.SetOwner(_trackLayout);
            track.Direction = track.GetBestTrackDirection(true);
            _trackRenderer.Render(canvas, track);
        }
    }
}
