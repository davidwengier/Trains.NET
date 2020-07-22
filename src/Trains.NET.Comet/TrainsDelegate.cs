using System;
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
    internal class TrainsDelegate : AbstractControlDelegate
    {
        private readonly IGame _game;
        private readonly IPixelMapper _pixelMapper;
        private readonly Factory<IToolPreviewer> _previewerFactory;
        private (int column, int row) _lastDragCell;
        private bool _dragging;
        private float _mouseX;
        private float _mouseY;
        private float _lastWidth;
        private float _lastHeight;
        private float _dpi = 1.25f;

        public State<ITool?> CurrentTool { get; } = new State<ITool?>();

        public TrainsDelegate(IGame game, IPixelMapper pixelMapper, Factory<IToolPreviewer> previewerFactory)
        {
            _game = game;
            _pixelMapper = pixelMapper;
            _previewerFactory = previewerFactory;
        }

        public override void Resized(RectangleF bounds)
        {
            _lastWidth = bounds.Width;
            _lastHeight = bounds.Height;
            _game.SetSize((int)(bounds.Width * _dpi), (int)(bounds.Height * _dpi));
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
        public override void Draw(SkiaSharp.SKCanvas canvas, RectangleF dirtyRect)
        {
            // Pull DPI from caller scale
            float newDpi = canvas.TotalMatrix.ScaleX;
            if(float.IsFinite(newDpi) && Math.Abs(newDpi - _dpi) > 0.0001)
            { 
                _dpi = newDpi;
                _game.SetSize((int)(_lastWidth * _dpi), (int)(_lastHeight * _dpi));
            }
            
            canvas.RestoreToCount(-1);

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

            if (this.CurrentTool.Value != null)
            {
                var previewer = _previewerFactory.Get(this.CurrentTool.Value.GetType());
                if (previewer != null)
                {
                    canvas.Save();
                    (int col, int row) = _pixelMapper.ViewPortPixelsToCoords((int)_mouseX, (int)_mouseY);
                    (int x, int y) = _pixelMapper.CoordsToViewPortPixels(col, row);
                    canvas.Translate(x, y);
                    previewer.RenderPreview(new SKCanvasWrapper(canvas), col, row);
                    canvas.Restore();
                }
            }
        }

        private (float x, float y) ConvertDPIScaledPointToRawPosition(PointF point) => (point.X * _dpi, point.Y * _dpi);

        public override void HoverInteraction(PointF[] points)
        {
            (float x, float y) = ConvertDPIScaledPointToRawPosition(points[0]);
            SetMousePosition(x, y);
        }

        private void SetMousePosition(float x, float y)
        {
            _mouseX = x;
            _mouseY = y;
        }

        public override bool StartInteraction(PointF[] points)
        {
            (float x, float y) = ConvertDPIScaledPointToRawPosition(points[0]);
            SetMousePosition(x, y);

            if (this.CurrentTool.Value == null)
            {
                return false;
            }

            (int column, int row) = _pixelMapper.ViewPortPixelsToCoords((int)x, (int)y);
            _lastDragCell = (column, row);
            if (this.CurrentTool.Value.IsValid(column, row) == true)
            {
                this.CurrentTool.Value.Execute(column, row);
            }
            else if (this.CurrentTool.Value is IDraggableTool tool)
            {
                tool.StartDrag((int)x, (int)y);
                _dragging = true;
            }
            return true;
        }

        public override void DragInteraction(PointF[] points)
        {
            (float x, float y) = ConvertDPIScaledPointToRawPosition(points[0]);
            SetMousePosition(x, y);

            if (this.CurrentTool.Value == null)
            {
                return;
            }

            if (_dragging && this.CurrentTool.Value is IDraggableTool tool)
            {
                tool.ContinueDrag((int)x, (int)y);
            }
            else
            {
                _dragging = false;
                (int column, int row) = _pixelMapper.ViewPortPixelsToCoords((int)x, (int)y);
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
            (float x, float y) = ConvertDPIScaledPointToRawPosition(points[0]);
            SetMousePosition(x, y);

            _lastDragCell = (-1, -1);
            _dragging = false;
        }
    }
}
