using System.Drawing;
using Comet;
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
        private (int column, int row) _lastDragCell;

        public State<ITool> CurrentTool { get; } = new State<ITool>();

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
            if (this.CurrentTool.Value == null)
            {
                return false;
            }

            (int column, int row) = _pixelMapper.PixelsToCoords((int)points[0].X, (int)points[0].Y);
            _lastDragCell = (column, row);
            if (this.CurrentTool.Value.IsValid(column, row) == true)
            {
                this.CurrentTool.Value.Execute(column, row);
            }

            return true;
        }

        public override void DragInteraction(PointF[] points)
        {
            if (this.CurrentTool.Value == null)
            {
                return;
            }

            (int column, int row) = _pixelMapper.PixelsToCoords((int)points[0].X, (int)points[0].Y);
            if (_lastDragCell == (column, row))
            {
                return;
            }

            _lastDragCell = (column, row);
            if (this.CurrentTool.Value.IsValid(column, row) == true)
            {
                this.CurrentTool.Value.Execute(column, row);
            }
        }

        public override void EndInteraction(PointF[] points)
        {
            _lastDragCell = (-1, -1);
        }
    }
}
