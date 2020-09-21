using System;
using System.Collections.Generic;
using Trains.NET.Engine;
using Trains.NET.Instrumentation;
using Trains.NET.Rendering.UI;

namespace Trains.NET.Rendering
{
    // This could totally be a screen, but layer renderers are drawn every frame
    // so its easier for it not to be.
    [Order(1000)]
    public class DiagnosticsRenderer : ILayerRenderer
    {
        public bool Enabled { get; set; }

        public string Name => "Diagnostics";

        public void Render(ICanvas canvas, int width, int height, IPixelMapper pixelMapper)
        {
            var strings = new List<string>();

            float maxWidth = 0;
            foreach ((string name, IStat stat) in InstrumentationBag.Stats)
            {
                if (stat.ShouldShow())
                {
                    string line = name + ": " + stat.GetDescription();
                    strings.Add(line);
                    maxWidth = Math.Max(maxWidth, canvas.MeasureText(line, Brushes.Label));
                }
            }

            var lineGap = 3;
            var lineHeight = Brushes.Label.TextSize.GetValueOrDefault();
            var panelHeight = strings.Count * (lineHeight + lineGap);

            canvas.Translate(10, height - panelHeight - 40);

            canvas.DrawRect(0, 0, maxWidth, panelHeight, Brushes.PanelBackground);
            foreach (string? line in strings)
            {
                canvas.DrawText(line, 0, lineHeight, Brushes.Label);
                canvas.Translate(0, lineGap + lineHeight);
            }
        }
    }
}
