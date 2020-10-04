using System;
using System.Collections.Generic;
using Trains.NET.Engine;

namespace Trains.NET.Rendering.UI
{
    public class TestPanelToggle : ITogglable
    {
        private bool _enabled;

        public event EventHandler? Changed;

        public string Name => "Test UI Panels";
        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                _enabled = value;
                Changed?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public class TestPanel : ButtonPanelBase
    {
        protected override int Top => 200;
        protected override string? Title => "Hi There";
        protected override bool IsCollapsable => true;

        private readonly Button _button = new Button("OMG! Awesome", () => false, () => { });

        public TestPanel(TestPanelToggle toggle)
        {
            this.Visible = toggle.Enabled;
            toggle.Changed += (s, e) => base.Visible = toggle.Enabled;
        }

        protected override IEnumerable<Button> GetButtons()
        {
            yield return _button;
        }
    }

    public class TestPanel1 : ButtonPanelBase
    {
        protected override int Top => 400;
        protected override int Left => 200;
        protected override string? Title => "Hi There";
        protected override bool IsCollapsable => true; // this should have no effect
        protected override PanelPosition Position => PanelPosition.Floating;

        private readonly Button _button = new Button("OMG! Awesome", () => false, () => { });

        public TestPanel1(TestPanelToggle toggle)
        {
            this.Visible = toggle.Enabled;
            toggle.Changed += (s, e) => base.Visible = toggle.Enabled;
        }

        protected override IEnumerable<Button> GetButtons()
        {
            yield return _button;
        }
    }

    public class TestPanel2 : PanelBase
    {
        protected override int Top => 400;
        protected override string? Title => "Hi There";
        protected override bool IsCollapsable => true;

        public TestPanel2(TestPanelToggle toggle)
        {
            this.Visible = toggle.Enabled;
            toggle.Changed += (s, e) => base.Visible = toggle.Enabled;
        }

        protected override void Render(ICanvas canvas)
        {
            canvas.DrawRect(0, 0, this.InnerWidth, this.InnerHeight, Brushes.Red);
        }
    }

    public class TestPanel3 : PanelBase
    {
        protected override int Top => 50;
        protected override string? Title => "Hi There";
        protected override PanelPosition Position => PanelPosition.Right;

        public TestPanel3(TestPanelToggle toggle)
        {
            this.Visible = toggle.Enabled;
            toggle.Changed += (s, e) => base.Visible = toggle.Enabled;
        }

        protected override void Render(ICanvas canvas)
        {
            canvas.DrawRect(0, 0, this.InnerWidth, this.InnerHeight, Brushes.Red);
        }
    }

    public class TestPanel4 : PanelBase
    {
        protected override int Left => 500;
        protected override int Top => 50;
        protected override string? Title => "Hi There";
        protected override PanelPosition Position => PanelPosition.Floating;

        public TestPanel4(TestPanelToggle toggle)
        {
            this.Visible = toggle.Enabled;
            toggle.Changed += (s, e) => base.Visible = toggle.Enabled;
        }

        protected override void Render(ICanvas canvas)
        {
            canvas.DrawRect(0, 0, this.InnerWidth, this.InnerHeight, Brushes.Red);
        }
    }

    public class TestPanel5 : PanelBase
    {
        protected override int Left => 700;
        protected override int Top => 50;
        protected override string? Title => "Hi There";
        protected override PanelPosition Position => PanelPosition.Floating;

        public TestPanel5(TestPanelToggle toggle)
        {
            this.Visible = toggle.Enabled;
            toggle.Changed += (s, e) => base.Visible = toggle.Enabled;
        }

        protected override void CalculateSize(ICanvas canvas)
        {
            this.InnerWidth = 20;
            this.InnerHeight = 20;
        }

        protected override void Render(ICanvas canvas)
        {
            canvas.DrawRect(0, 0, this.InnerWidth, this.InnerHeight, Brushes.Red);
        }
    }

    public class TestPanel6 : PanelBase
    {
        protected override int Left => 700;
        protected override int Top => 150;
        protected override PanelPosition Position => PanelPosition.Floating;

        public TestPanel6(TestPanelToggle toggle)
        {
            this.Visible = toggle.Enabled;
            toggle.Changed += (s, e) => base.Visible = toggle.Enabled;
        }

        protected override void CalculateSize(ICanvas canvas)
        {
            this.InnerWidth = 20;
            this.InnerHeight = 20;
        }

        protected override void Render(ICanvas canvas)
        {
            canvas.DrawRect(0, 0, this.InnerWidth, this.InnerHeight, Brushes.Red);
        }
    }
}
