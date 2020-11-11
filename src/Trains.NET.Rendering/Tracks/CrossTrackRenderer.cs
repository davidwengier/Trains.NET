using Trains.NET.Engine;

namespace Trains.NET.Rendering.Tracks
{
    public class CrossTrackRenderer : SpecializedEntityRenderer<CrossTrack, Track>
    {
        private readonly TrackRenderer _trackRenderer;

        public CrossTrackRenderer(TrackRenderer trackRenderer)
        {
            _trackRenderer = trackRenderer;
        }

        protected override void Render(ICanvas canvas, CrossTrack item)
        {
            _trackRenderer.DrawCross(canvas);
        }
    }
}
