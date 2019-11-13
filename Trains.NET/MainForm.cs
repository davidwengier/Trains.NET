using System;
using System.Collections;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using Trains.NET.Engine;
using Trains.NET.Rendering;

namespace Trains.NET
{
    public partial class MainForm : Form
    {
        private readonly Game _game;

        public MainForm()
        {
            this.Text = "Trains.NET";
            this.AutoScaleMode = AutoScaleMode.Font;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(1086, 559);
            this.ClientSize = new System.Drawing.Size(1547, 897);

            var splitContainer = new SplitContainer()
            {
                FixedPanel = FixedPanel.Panel1,
                SplitterDistance = 400,
                Dock = DockStyle.Fill,
                IsSplitterFixed = true
            };

            var buttonPanel = new Panel()
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(5)
            };

            foreach (Tool tool in ((Tool[])Enum.GetValues(typeof(Tool))).Reverse())
            {
                buttonPanel.Controls.Add(CreateButton(tool));
            }

            _game = new Game(new GameBoard());

            var skiaView = new SKControl()
            {
                Dock = DockStyle.Fill
            };

            skiaView.MouseDown += DoMouseClick;
            skiaView.Resize += (s, e) => _game.SetSize(skiaView.Width, skiaView.Height);
            skiaView.PaintSurface += (s, e) => _game.Render(e.Surface);

            splitContainer.Panel1.Controls.Add(buttonPanel);
            splitContainer.Panel2.Controls.Add(skiaView);
            splitContainer.Panel2.Padding = new Padding(5);

            this.Controls.Add(splitContainer);

            void DoMouseClick(object sender, MouseEventArgs e)
            {
                if ((e.Button & MouseButtons.Left) != MouseButtons.Left) return;

                _game.OnMouseDown(e.X, e.Y);

                skiaView.Refresh();
            }

            RadioButton CreateButton(Tool tool)
            {
                var button = new RadioButton()
                {
                    Text = tool.ToString(),
                    Dock = DockStyle.Top,
                    Height = 50,
                    Appearance = Appearance.Button,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Checked = tool == (Tool)0
                };

                button.Click += (s, e) => _game.CurrentTool = tool;

                return button;
            }
        }
    }
}
