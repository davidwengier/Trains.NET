using System.Collections.Generic;

namespace Trains.NET.Rendering.HtmlCanvas
{
    internal class PathWrapper : IPath
    {
        private readonly List<(string, object[])> _commands = new List<(string, object[])>();

        public IEnumerable<(string, object[])> Commands => _commands;

        public void ArcTo(float radiusX, float radiusY, int xAxisRotate, PathArcSize arcSize, PathDirection direction, float x, int y)
        {
            _commands.Add(("canvas.arcTo", new object[] { x, y, radiusX, direction == PathDirection.CounterClockwise }));
        }

        public void LineTo(float x, float y)
        {
            _commands.Add(("canvas.lineTo", new object[] { x, y }));
        }

        public void MoveTo(float x, float y)
        {
            _commands.Add(("canvas.moveTo", new object[] { x, y }));
        }
    }
}
