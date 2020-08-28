using System.Drawing;
using System.Windows;
using System.Windows.Input;
using Comet;
using Comet.Skia;
using Trains.NET.Engine;
using Trains.NET.Rendering;
using Trains.NET.Rendering.Skia;
using Trains.NET.Rendering.Tracks;

namespace Trains.NET.Comet
{
    public class TrainsDelegate : AbstractControlDelegate
    {
        private readonly IGame _game;
        private readonly IPixelMapper _pixelMapper;
        private readonly Factory<IToolPreviewer> _previewerFactory;
        private (int column, int row) _lastDragCell;
        private bool _dragging;
        private float _mouseX;
        private float _mouseY;

        public State<ITool?> CurrentTool { get; } = new State<ITool?>();

        public TrainsDelegate(IGame game, IPixelMapper pixelMapper, Factory<IToolPreviewer> previewerFactory)
        {
            _game = game;
            _pixelMapper = pixelMapper;
            _previewerFactory = previewerFactory;
        }

        public override void Resized(RectangleF bounds)
        {
            _game.SetSize((int)bounds.Width, (int)bounds.Height);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
        public override void Draw(SkiaSharp.SKCanvas canvas, RectangleF dirtyRect)
        {
            _game.Render(new SKCanvasWrapper(canvas));

            if (this.CurrentTool.Value is ICustomCursor cursor)
            {
                if (this.NativeDrawableControl is FrameworkElement element)
                {
                    element.Cursor = Cursors.None;
                }

                canvas.Save();
                canvas.Translate((int)_mouseX, (int)_mouseY);
                cursor.RenderCursor(new SKCanvasWrapper(canvas));
                canvas.Restore();
            }
            else
            {
                if (this.NativeDrawableControl is FrameworkElement element)
                {
                    element.Cursor = Cursors.Arrow;
                }
            }

            if (this.CurrentTool.Value is not null)
            {
                (int col, int row) = _pixelMapper.ViewPortPixelsToCoords((int)_mouseX, (int)_mouseY);
                IToolPreviewer? previewer = _previewerFactory.Get(this.CurrentTool.Value.GetType());
                if (previewer is not null && this.CurrentTool.Value.IsValid(col, row))
                {
                    canvas.Save();
                    (int x, int y, _) = _pixelMapper.CoordsToViewPortPixels(col, row);
                    canvas.Translate(x, y);
                    previewer.RenderPreview(new SKCanvasWrapper(canvas), col, row);
                    canvas.Restore();
                }
            }
        }

        public override void HoverInteraction(PointF[] points)
        {
            SetMousePosition(points);
        }

        private void SetMousePosition(PointF[] points)
        {
            _mouseX = points[0].X;
            _mouseY = points[0].Y;
        }

        public override bool StartInteraction(PointF[] points)
        {
            SetMousePosition(points);

            if (this.CurrentTool.Value == null)
            {
                return false;
            }

            (int column, int row) = _pixelMapper.ViewPortPixelsToCoords((int)points[0].X, (int)points[0].Y);
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
            SetMousePosition(points);

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
                (int column, int row) = _pixelMapper.ViewPortPixelsToCoords((int)points[0].X, (int)points[0].Y);
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
            SetMousePosition(points);

            _lastDragCell = (-1, -1);
            _dragging = false;
        }
    }
}
