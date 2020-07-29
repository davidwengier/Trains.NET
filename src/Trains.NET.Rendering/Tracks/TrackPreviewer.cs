using Trains.NET.Engine;

namespace Trains.NET.Rendering.Tracks
{
    [Context(typeof(TrackTool))]
    internal class TrackPreviewer : IToolPreviewer
    {
        private readonly IStaticEntityCollection _trackLayout;
        private readonly ITrackRenderer _trackRenderer;

        public TrackPreviewer(IStaticEntityCollection trackLayout, ITrackRenderer trackRenderer)
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
