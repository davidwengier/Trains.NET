using Trains.NET.Engine;
using Trains.NET.Instrumentation;

namespace Trains.NET.Rendering
{
    [Order(1000)]
    internal class DiagnosticsRenderer : ILayerRenderer
    {
        private readonly PaintBrush _paint = new PaintBrush
        {
            Color = Colors.Black,
            TextSize = 16,
            TextAlign = TextAlign.Left,
        };

        public bool Enabled { get; set; }

        public string Name => "Diagnostics";

        public DiagnosticsRenderer()
        {
        }

        public void Render(ICanvas canvas, int width, int height)
        {
            int i = 1;

            foreach((string name, IStat stat) in InstrumentationBag.Stats)
            {
                if (stat.ShouldShow())
                {
                    canvas.DrawText(name + ": " + stat.GetDescription(), 0, i++ * 25, _paint);
                }
            }
        }
    }
}
