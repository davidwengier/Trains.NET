using System;
using System.Collections.Generic;
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
        private readonly PaintBrush _backgroundPaint = new PaintBrush
        {
            Color = new Color("#DDFFFFFF"),
            Style = PaintStyle.Fill
        };

        public bool Enabled { get; set; }

        public string Name => "Diagnostics";

        public DiagnosticsRenderer()
        {
        }

        public void Render(ICanvas canvas, int width, int height, IPixelMapper pixelMapper)
        {
            int i = 1;

            var strings = new List<string>();

            float maxWidth = 0;
            foreach((string name, IStat stat) in InstrumentationBag.Stats)
            {
                if (stat.ShouldShow())
                {
                    string line = name + ": " + stat.GetDescription();
                    strings.Add(line);
                    maxWidth = Math.Max(maxWidth, canvas.MeasureText(line, _paint));
                }
            }

            canvas.DrawRect(0, 0, maxWidth, strings.Count * 26, _backgroundPaint);
            foreach (string? line in strings)
            {
                canvas.DrawText(line, 0, i++ * 25, _paint);
            }
        }
    }
}
