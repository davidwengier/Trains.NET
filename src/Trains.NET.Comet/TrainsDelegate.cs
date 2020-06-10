using System.Drawing;
using Comet;
using Comet.Skia;
using Trains.NET.Engine;
using Trains.NET.Rendering;
using Trains.NET.Rendering.Skia;

namespace Trains.NET.Comet
{
    internal class TrainsDelegate : AbstractControlDelegate
    {
        private readonly IGame _game;
        private readonly IPixelMapper _pixelMapper;
        private (int column, int row) _lastDragCell;
        private bool _dragging;

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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
        public override void Draw(SkiaSharp.SKCanvas canvas, RectangleF dirtyRect)
        {
            _game.Render(new SKCanvasWrapper(canvas));
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
            else if (this.CurrentTool.Value is IDraggableTool tool)
            {
                tool.StartDrag((int)points[0].X, (int)points[0].Y);
                _dragging = true;
            }
            return true;
        }

        public override void DragInteraction(PointF[] points)
        {
            if (this.CurrentTool.Value == null)
            {
                return;
            }

            if (_dragging && this.CurrentTool.Value is IDraggableTool tool)
            {
                tool.ContinueDrag((int)points[0].X, (int)points[0].Y);
            }
            else
            {
                _dragging = false;
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
        }

        public override void EndInteraction(PointF[] points)
        {
            _lastDragCell = (-1, -1);
            _dragging = false;
        }
    }
}
