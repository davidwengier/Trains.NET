using System.Threading.Tasks;
using Microsoft.JSInterop;
using Trains.NET.Engine;

namespace Trains.NET.Rendering.HtmlCanvas
{
    public class CanvasWrapper : ICanvas
    {
        private string? _id;
        private IJSRuntime? _jsRuntime;
        public int Width { get; private set; }
        public int Height { get; private set; }

        public async Task InitAsync(string id, IJSRuntime jSRuntime)
        {
            _id = id;
            _jsRuntime = jSRuntime;

            await _jsRuntime.InvokeVoidAsync("canvas.init", id);

            this.Width = await _jsRuntime.InvokeAsync<int>("canvas.getWidth");
            this.Height = await _jsRuntime.InvokeAsync<int>("canvas.getHeight");
        }

        public void Clear(Colors color)
        {
            _jsRuntime.InvokeVoidAsync("canvas.clear", color.ToString());
        }

        public void ClipRect(Rectangle rect, ClipOperation operation, bool antialias)
        {
            _jsRuntime.InvokeVoidAsync("canvas.clipRect", rect.Left, rect.Top, rect.Right, rect.Bottom);
        }

        public void DrawCircle(float x, float y, float radius, PaintBrush paint)
        {
            _jsRuntime.InvokeVoidAsync("canvas.drawCircle", x, y, radius, paint.StrokeWidth, paint.Color.ToString());
        }

        public void DrawLine(float x1, float y1, float x2, float y2, PaintBrush paint)
        {
            _jsRuntime.InvokeVoidAsync("canvas.drawLine", x1, y1, x2, y2, paint.StrokeWidth, paint.Color.ToString());
        }

        public void DrawPath(IPath trackPath, PaintBrush paint)
        {
            var commands = ((PathWrapper)trackPath).Commands;

            _jsRuntime.InvokeVoidAsync("canvas.beginPath");
            foreach (var command in commands)
            {
                _jsRuntime.InvokeVoidAsync(command.Item1, command.Item2);
            }
            _jsRuntime.InvokeVoidAsync("canvas.drawPath", paint.StrokeWidth, paint.Color.ToString());
        }

        public void DrawRect(float x, float y, float width, float height, PaintBrush paint)
        {
            _jsRuntime.InvokeVoidAsync("canvas.drawRect", x, y, width, height, paint.StrokeWidth, paint.Color.ToString());
        }

        public void DrawText(string text, float x, float y, PaintBrush paint)
        {
            _jsRuntime.InvokeVoidAsync("canvas.drawText", text, x, y, paint.TextSize, paint.TextAlign.ToString().ToLowerInvariant(), paint.Color.ToString());
        }

        public void Restore()
        {
            _jsRuntime.InvokeVoidAsync("canvas.restore");
        }

        public void RotateDegrees(float degrees, float x, float y)
        {
            _jsRuntime.InvokeVoidAsync("canvas.rotate", TrainMovement.DegreeToRad(degrees), x, y);
        }

        public void RotateDegrees(float degrees)
        {
            _jsRuntime.InvokeVoidAsync("canvas.rotate", TrainMovement.DegreeToRad(degrees));
        }

        public void Save()
        {
            _jsRuntime.InvokeVoidAsync("canvas.save");
        }

        public void Translate(float x, float y)
        {
            _jsRuntime.InvokeVoidAsync("canvas.translate", x, y);
        }
    }
}
