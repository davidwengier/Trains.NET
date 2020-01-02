using System.Drawing;
using Comet.Skia;
using SkiaSharp;
using Trains.NET.Rendering;

namespace Trains.NET.Comet
{
    internal class TrainsDelegate : AbstractControlDelegate
    {
        private readonly IGame _game;

        public TrainsDelegate(IGame game)
        {
            _game = game;
        }

        public override void Resized(RectangleF bounds)
        {
            _game.SetSize((int)bounds.Width, (int)bounds.Height);
        }

        public override void Draw(SKCanvas canvas, RectangleF dirtyRect)
        {
            _game.Render(canvas);
        }
    }
}
