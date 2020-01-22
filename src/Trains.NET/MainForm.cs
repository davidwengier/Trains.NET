using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SkiaSharp.Views.Desktop;
using Trains.NET.Engine;
using Trains.NET.Rendering;

namespace Trains.NET
{
    public partial class MainForm : Form
    {
        private readonly IGame _game;
        private readonly ITrackParameters _parameters;
        private readonly Timer _renderLoopTimer;
        private Form? _debugForm;
        private ITool _currentTool;

        public MainForm(IGame game,
                        IPixelMapper pixelMapper,
                        IEnumerable<ITool> tools,
                        IEnumerable<ILayerRenderer> renderers,
                        ITrackParameters parameters)
        {
            _game = game;
            _parameters = parameters;
            _currentTool = tools.First();

            this.Text = "Trains.NET";
            this.AutoScaleMode = AutoScaleMode.Font;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(1086, 559);
            this.ClientSize = new Size(1547, 897);

            var splitContainer = new SplitContainer()
            {
                FixedPanel = FixedPanel.Panel1,
                SplitterDistance = 400,
                Dock = DockStyle.Fill,
                IsSplitterFixed = true
            };

            var buttonPanel = new Panel()
            {
                Dock = DockStyle.Top,
                Padding = new Padding(5),
                AutoSize = true
            };

            var rendererPanel = new Panel()
            {
                Dock = DockStyle.Top,
                Padding = new Padding(5),
                AutoSize = true
            };

            foreach (ITool tool in tools)
            {
                buttonPanel.Controls.Add(CreateButton(tool));
            }

            var button = new CheckBox
            {
                Text = "Configure",
                Height = 30,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Appearance = Appearance.Button
            };

            buttonPanel.Controls.Add(button);

            button.CheckedChanged += (s, e) =>
            {
                if (_debugForm == null)
                {
                    _debugForm = CreateDebugForm();
                }

                if (button.Checked)
                {
                    _debugForm.Location = new Point(this.Left - _debugForm.Width, this.Top);
                    _debugForm.Show();
                }
                else
                {
                    _debugForm.Hide();
                }
            };

            var skiaView = new SKControl()
            {
                Dock = DockStyle.Fill
            };

            skiaView.MouseDown += DoMouseClick;
            skiaView.MouseMove += DoMouseClick;
            skiaView.Resize += (s, e) => _game.SetSize(skiaView.Width, skiaView.Height);
            skiaView.PaintSurface += (s, e) => _game.Render(e.Surface.Canvas);

            foreach (ILayerRenderer renderer in renderers)
            {
                rendererPanel.Controls.Add(CreateRendererCheckbox(renderer));
            }

            splitContainer.Panel1.Controls.Add(rendererPanel);
            splitContainer.Panel1.Controls.Add(buttonPanel);
            splitContainer.Panel2.Controls.Add(skiaView);
            splitContainer.Panel2.Padding = new Padding(5);

            _renderLoopTimer = new Timer();
            _renderLoopTimer.Tick += (s, e) => skiaView.Refresh();
            _renderLoopTimer.Interval = 16;
            _renderLoopTimer.Start();

            this.Controls.Add(splitContainer);

            void DoMouseClick(object sender, MouseEventArgs e)
            {
                if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
                {
                    (int column, int row) = pixelMapper.PixelsToCoords(e.X, e.Y);
                    _currentTool.Execute(column, row);
                }
            }

            RadioButton CreateButton(ITool tool)
            {
                var button = new RadioButton()
                {
                    Text = tool.Name,
                    Dock = DockStyle.Top,
                    Height = 50,
                    Appearance = Appearance.Button,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Checked = _currentTool == tool
                };

                button.Click += (s, e) => _currentTool = tool;

                return button;
            }

            Control CreateRendererCheckbox(ILayerRenderer renderer)
            {
                var checkbox = new CheckBox
                {
                    Text = renderer.Name,
                    Dock = DockStyle.Top,
                    Height = 50,
                    Appearance = Appearance.Button,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Checked = renderer.Enabled
                };

                checkbox.CheckedChanged += (s, e) =>
                {
                    renderer.Enabled = checkbox.Checked;
                };

                return checkbox;
            }
        }

        private Form CreateDebugForm()
        {
            var f = new Form()
            {
                FormBorderStyle = FormBorderStyle.SizableToolWindow,
                Text = "Configuration",
                Width = 200,
                Height = this.Height,
                StartPosition = FormStartPosition.Manual,
            };

            var panel = new FlowLayoutPanel()
            {
                FlowDirection = FlowDirection.TopDown,
                Dock = DockStyle.Fill
            };

            SetupControls(panel, () => _parameters.CellSize, v => _parameters.CellSize = v, nameof(_parameters.CellSize), 100);
            SetupControls(panel, () => _parameters.NumPlanks, v => _parameters.NumPlanks = v, nameof(_parameters.NumPlanks), 10);
            SetupControls(panel, () => _parameters.NumCornerPlanks, v => _parameters.NumCornerPlanks = v, nameof(_parameters.NumCornerPlanks), 10);
            SetupControls(panel, () => _parameters.PlankWidth, v => _parameters.PlankWidth = v, nameof(_parameters.PlankWidth), 20);
            SetupControls(panel, () => _parameters.PlankPadding, v => _parameters.PlankPadding = v, nameof(_parameters.PlankPadding), 50);
            SetupControls(panel, () => _parameters.TrackPadding, v => _parameters.TrackPadding = v, nameof(_parameters.TrackPadding), 50);
            SetupControls(panel, () => _parameters.TrackWidth, v => _parameters.TrackWidth = v, nameof(_parameters.TrackWidth), 10);
            SetupControls(panel, () => _parameters.CornerStepDegrees, v => _parameters.CornerStepDegrees = v, nameof(_parameters.CornerStepDegrees), 90);
            SetupControls(panel, () => _parameters.CornerEdgeOffsetDegrees, v => _parameters.CornerEdgeOffsetDegrees = v, nameof(_parameters.CornerEdgeOffsetDegrees), 90);

            f.Controls.Add(panel);
            return f;
        }

        private static void SetupControls(FlowLayoutPanel panel, Func<int> getter, Action<int> setter, string name, int max)
        {
            var lbl = new Label
            {
                Text = name + ":",
                AutoSize = true
            };
            panel.Controls.Add(lbl);

            var slider = new TrackBar()
            {
                Minimum = 1,
                Maximum = max,
                Value = getter()
            };

            slider.ValueChanged += (s, e) =>
            {
                setter(slider.Value);
            };
            panel.Controls.Add(slider);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            _debugForm?.Dispose();
            _renderLoopTimer.Dispose();
        }
    }
}
