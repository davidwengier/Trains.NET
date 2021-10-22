using System;
using System.Collections.Generic;
using System.Linq;
using Trains.NET.Engine;
using Trains.NET.Instrumentation;
using Trains.NET.Rendering.UI;

namespace Trains.NET.Rendering
{
    [Order(1000)]
    public class DiagnosticsPanel : PanelBase, ITogglable
    {
        private const int LineGap = 3;

        public bool Enabled
        {
            get { return this.Visible; }
            set
            {
                this.Visible = value;
                OnChanged();
            }
        }

        public string Name => "Diagnostics";

        protected override PanelPosition Position => PanelPosition.Floating;
        protected override int BottomPadding => 2;
        protected override int TopPadding => 2;
        protected override int CornerRadius => 2;

        protected override PaintBrush PanelBorderBrush { get; } = Brushes.PanelBorder with { StrokeWidth = 1 };

        public DiagnosticsPanel()
        {
            this.Left = 10;
            this.Visible = false;
        }

        protected override void PreRender(ICanvas canvas)
        {
            this.Top = int.MaxValue;
        }

        protected override void Render(ICanvas canvas)
        {
            var lineHeight = Brushes.Label.TextSize.GetValueOrDefault();

            float maxWidth = 0;
            var strings = new List<string>();
            foreach ((string name, IStat stat) in InstrumentationBag.Stats.OrderBy(i => i.Name))
            {
                if (stat.ShouldShow())
                {
                    string line = name + ": " + stat.GetDescription();
                    strings.Add(line);
                    maxWidth = Math.Max(maxWidth, canvas.MeasureText(line, Brushes.Label));
                }
            }

            this.InnerWidth = (int)maxWidth;
            this.InnerHeight = strings.Count * (lineHeight + LineGap);

            foreach (string line in strings)
            {
                canvas.DrawText(line, 0, lineHeight, Brushes.Label);
                canvas.Translate(0, LineGap + lineHeight);
            }
        }
    }
}
