using System.Drawing;
using Comet.Skia;
using Trains.NET.Rendering;
using Trains.NET.Rendering.Skia;

namespace Trains.NET.Comet
{
    public class TrainsDelegate : AbstractControlDelegate
    {
        private readonly IGame _game;
        private readonly IInteractionManager _interactionManager;

        public TrainsDelegate(IGame game, IInteractionManager interactionManager)
        {
            _game = game;
            _interactionManager = interactionManager;
        }

        public override void Resized(RectangleF bounds)
        {
            _game.SetSize((int)bounds.Width, (int)bounds.Height);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
        public override void Draw(SkiaSharp.SKCanvas canvas, RectangleF dirtyRect)
        {
            _game.Render(new SKCanvasWrapper(canvas));
        }

        public override bool StartInteraction(PointF[] points)
            => _interactionManager.PointerClick((int)points[0].X, (int)points[0].Y);

        public override void StartHoverInteraction(PointF[] points)
            => _interactionManager.PointerMove((int)points[0].X, (int)points[0].Y);

        public override void DragInteraction(PointF[] points)
            => _interactionManager.PointerDrag((int)points[0].X, (int)points[0].Y);

        public override void EndInteraction(PointF[] points)
            => _interactionManager.PointerRelease((int)points[0].X, (int)points[0].Y);
    }
}
