using System.Drawing;
using Comet.Skia;
using SkiaSharp;
using Trains.NET.Engine;
using Trains.NET.Rendering;

namespace Trains.NET.Comet
{
    internal class TrainsDelegate : AbstractControlDelegate
    {
        private readonly IGame _game;
        private readonly IPixelMapper _pixelMapper;

        public ITool? CurrentTool { get; set; }

        public TrainsDelegate(IGame game, IPixelMapper pixelMapper)
        {
            _game = game;
            _pixelMapper = pixelMapper;
        }

        public override void Resized(RectangleF bounds)
        {
            _game.SetSize((int)bounds.Width, (int)bounds.Height);
        }

        public override void Draw(SKCanvas canvas, RectangleF dirtyRect)
        {
            _game.Render(canvas);
        }

        public override bool StartInteraction(PointF[] points)
        {
            (int column, int row) = _pixelMapper.PixelsToCoords((int)points[0].X, (int)points[0].Y);
            this.CurrentTool?.Execute(column, row);

            Invalidate();

            return true;
        }

        public override void DragInteraction(PointF[] points)
        {
            (int column, int row) = _pixelMapper.PixelsToCoords((int)points[0].X, (int)points[0].Y);
            this.CurrentTool?.Execute(column, row);

            Invalidate();
        }
    }
}
