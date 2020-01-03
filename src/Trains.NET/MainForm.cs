using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SkiaSharp.Views.Desktop;
using Trains.NET.Rendering;

namespace Trains.NET
{
    public partial class MainForm : Form
    {
        private readonly IGame _game;
        private readonly ITrackParameters _parameters;
        private readonly SKControl _skiaView;
        private Form? _debugForm;

        public MainForm(IGame game, IEnumerable<IBoardRenderer> renderers, ITrackParameters parameters)
        {
            _game = game;
            _parameters = parameters;
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

            foreach (Tool tool in ((Tool[])Enum.GetValues(typeof(Tool))).Reverse())
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

            _skiaView = new SKControl()
            {
                Dock = DockStyle.Fill
            };

            _skiaView.MouseDown += DoMouseClick;
            _skiaView.MouseMove += DoMouseClick;
            _skiaView.Resize += (s, e) => _game.SetSize(_skiaView.Width, _skiaView.Height);
            _skiaView.PaintSurface += (s, e) => _game.Render(e.Surface.Canvas);

            foreach (IBoardRenderer renderer in renderers)
            {
                rendererPanel.Controls.Add(CreateRendererCheckbox(renderer));
            }

            splitContainer.Panel1.Controls.Add(rendererPanel);
            splitContainer.Panel1.Controls.Add(buttonPanel);
            splitContainer.Panel2.Controls.Add(_skiaView);
            splitContainer.Panel2.Padding = new Padding(5);

            this.Controls.Add(splitContainer);

            void DoMouseClick(object sender, MouseEventArgs e)
            {
                if (e.Button == MouseButtons.None) return;
                if ((e.Button & MouseButtons.Middle) == MouseButtons.Middle) return;

                bool isRightMouseButton = (e.Button & MouseButtons.Right) == MouseButtons.Right;
                _game.OnMouseDown(e.X, e.Y, isRightMouseButton);

                _skiaView.Refresh();
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

            Control CreateRendererCheckbox(IBoardRenderer renderer)
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
                    _skiaView.Refresh();
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

        private void SetupControls(FlowLayoutPanel panel, Func<int> getter, Action<int> setter, string name, int max)
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
                _skiaView.Refresh();
            };
            panel.Controls.Add(slider);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            _debugForm?.Dispose();
            _skiaView.Dispose();
        }
    }
}
