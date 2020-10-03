#if DEBUG
using System.Collections.Generic;

namespace Trains.NET.Rendering.UI
{
    public class TestPanel : ButtonPanelBase
    {
        protected override int Top => 200;
        protected override string? Title => "Hi There";
        protected override bool IsCollapsable => true;

        protected override IEnumerable<Button> GetButtons()
        {
            yield return new Button("OMG! Awesome", () => false, () => { });
        }
    }

    public class TestPanel2 : PanelBase
    {
        protected override int Top => 400;
        protected override string? Title => "Hi There";
        protected override bool IsCollapsable => true;

        public override void Render(ICanvas canvas, int width, int height)
        {
            base.Render(canvas, width, height);

            canvas.DrawRect(0, 0, 100, 100, Brushes.Red);
        }
    }

    public class TestPanel3 : PanelBase
    {
        protected override int Top => 50;
        protected override string? Title => "Hi There";
        protected override PanelPosition Position => PanelPosition.Right;

        public override void Render(ICanvas canvas, int width, int height)
        {
            base.Render(canvas, width, height);

            canvas.DrawRect(0, 0, 100, 100, Brushes.Red);
        }
    }

    public class TestPanel4 : PanelBase
    {
        protected override int Left => 500;
        protected override int Top => 50;
        protected override string? Title => "Hi There";
        protected override PanelPosition Position => PanelPosition.Floating;

        public override void Render(ICanvas canvas, int width, int height)
        {
            base.Render(canvas, width, height);

            canvas.DrawRect(0, 0, 100, 100, Brushes.Red);
        }
    }

    public class TestPanel5 : PanelBase
    {
        protected override int Left => 700;
        protected override int Top => 50;
        protected override string? Title => "Hi There";
        protected override PanelPosition Position => PanelPosition.Floating;

        public override void Render(ICanvas canvas, int width, int height)
        {
            this.InnerWidth = 20;
            this.InnerHeight = 20;
            base.Render(canvas, width, height);

            canvas.DrawRect(0, 0, this.InnerWidth, this.InnerHeight, Brushes.Red);
        }
    }

    public class TestPanel6 : PanelBase
    {
        protected override int Left => 700;
        protected override int Top => 150;
        protected override PanelPosition Position => PanelPosition.Floating;

        public override void Render(ICanvas canvas, int width, int height)
        {
            this.InnerWidth = 20;
            this.InnerHeight = 20;
            base.Render(canvas, width, height);

            canvas.DrawRect(0, 0, this.InnerWidth, this.InnerHeight, Brushes.Red);
        }
    }
}

#endif
