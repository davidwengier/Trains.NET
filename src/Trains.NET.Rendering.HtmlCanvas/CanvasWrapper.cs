using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Trains.NET.Engine;

namespace Trains.NET.Rendering.HtmlCanvas
{
    public class CanvasWrapper : ICanvas
    {
        private int _saveStackPointer = 0;
        private readonly List<(string command, object[] args)> _commands = new List<(string, object[])>();

        private string? _id;
        private IJSRuntime? _jsRuntime;
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int ClientX { get; private set; }
        public int ClientY { get; private set; }

        public async Task InitAsync<T>(string id, IJSRuntime jSRuntime, DotNetObjectReference<T> razorView) where T : class
        {
            _id = id;
            _jsRuntime = jSRuntime;

            await _jsRuntime.InvokeVoidAsync("canvas.init", id, razorView);

            this.Width = await _jsRuntime.InvokeAsync<int>("canvas.getWidth");
            this.Height = await _jsRuntime.InvokeAsync<int>("canvas.getHeight");
            this.ClientX = (int)await _jsRuntime.InvokeAsync<double>("canvas.getClientX");
            this.ClientY = (int)await _jsRuntime.InvokeAsync<double>("canvas.getClientY");
        }

        private void AddCommand(string command, params object[] args)
        {
            _jsRuntime.InvokeVoidAsync(command, args);
            //_commands.Add((command, args));
        }

        private void RenderCommands()
        {
            //_jsRuntime.InvokeVoidAsync("canvas.render", System.Text.Json.JsonSerializer.Serialize(
            //    (from c in _commands
            //     select new
            //     {
            //         command = c.command,
            //         args = System.Text.Json.JsonSerializer.Serialize(c.args)
            //     }).ToArray()
            //    ));

        }

        public void Clear(Colors color)
        {
            AddCommand("canvas.clear", color.ToString());
        }

        public void ClipRect(Rectangle rect, ClipOperation operation, bool antialias)
        {
            AddCommand("canvas.clipRect", rect.Left, rect.Top, rect.Right, rect.Bottom);
        }

        public void DrawCircle(float x, float y, float radius, PaintBrush paint)
        {
            AddCommand("canvas.drawCircle", x, y, radius, paint.StrokeWidth, paint.Color.ToString());
        }

        public void DrawLine(float x1, float y1, float x2, float y2, PaintBrush paint)
        {
            AddCommand("canvas.drawLine", x1, y1, x2, y2, paint.StrokeWidth, paint.Color.ToString());
        }

        public void DrawPath(IPath trackPath, PaintBrush paint)
        {
            var commands = ((PathWrapper)trackPath).Commands;

            AddCommand("canvas.beginPath");
            foreach (var command in commands)
            {
                AddCommand(command.Item1, command.Item2);
            }
            AddCommand("canvas.drawPath", paint.StrokeWidth, paint.Color.ToString());
        }

        public void DrawRect(float x, float y, float width, float height, PaintBrush paint)
        {
            AddCommand("canvas.drawRect", x, y, width, height, paint.StrokeWidth, paint.Color.ToString(), paint.Style == PaintStyle.Fill);
        }

        public void DrawText(string text, float x, float y, PaintBrush paint)
        {
            AddCommand("canvas.drawText", text, x, y, paint.TextSize, paint.TextAlign.ToString().ToLowerInvariant(), paint.Color.ToString());
        }

        public void Restore()
        {
            _saveStackPointer--;
            AddCommand("canvas.restore");

            if (_saveStackPointer == 0)
            {
                RenderCommands();
            }
        }

        public void RotateDegrees(float degrees, float x, float y)
        {
            AddCommand("canvas.rotate", TrainMovement.DegreeToRad(degrees), x, y);
        }

        public void RotateDegrees(float degrees)
        {
            AddCommand("canvas.rotate", TrainMovement.DegreeToRad(degrees));
        }

        public void Save()
        {
            _saveStackPointer++;
            AddCommand("canvas.save");
        }

        public void Translate(float x, float y)
        {
            AddCommand("canvas.translate", x, y);
        }

        public void GradientRect(float x, float y, float width, float height, Colors start, Colors end)
        {
            throw new System.NotImplementedException();
        }
    }
}
