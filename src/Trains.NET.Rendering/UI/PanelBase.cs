using System;

namespace Trains.NET.Rendering.UI
{
    public abstract class PanelBase : IScreen
    {
        protected const int TitleAreaWidth = 20;
        public event EventHandler? Changed;

        protected bool Collapsed { get; set; }

        protected virtual bool IsCollapsable { get; }
        protected virtual string? Title { get; }
        protected abstract int Top { get; }
        protected virtual int TopPadding { get; } = 15;
        protected virtual int BottomPadding { get; } = 15;
        protected virtual PanelPosition Position { get; } = PanelPosition.Left;
        protected virtual int Left { get; }
        protected float TitleWidth { get; set; }
        protected float InnerWidth { get; set; } = 100;
        protected float InnerHeight { get; set; } = 100;

        protected void OnChanged()
        {
            Changed?.Invoke(this, EventArgs.Empty);
        }

        protected bool IsCollapsed()
            => this.IsCollapsable && this.Collapsed && this.Title is { Length: > 0 } && this.Position != PanelPosition.Floating;


        public virtual bool HandleInteraction(int x, int y, int width, int height, MouseAction action)
        {
            return false;
        }

        public virtual void Render(ICanvas canvas, int width, int height)
        {
            canvas.Translate(0, this.Top);

            this.TitleWidth = 0f;
            if (this.Title is { Length: > 0 })
            {
                this.TitleWidth = canvas.MeasureText(this.Title, Brushes.Label);
            }

            var panelHeight = Math.Max(this.TitleWidth, this.InnerHeight) + this.TopPadding + this.BottomPadding;
            var panelWidth = this.InnerWidth + 20;
            if (this.Position != PanelPosition.Floating)
            {
                panelWidth += 20;
            }

            if (this.Position == PanelPosition.Right)
            {
                if (IsCollapsed())
                {
                    canvas.Translate(width + 2, 0);
                }
                else
                {
                    canvas.Translate(width - panelWidth + 20, 0);
                }
            }
            else if (this.Position == PanelPosition.Left)
            {
                if (IsCollapsed())
                {
                    canvas.Translate(-panelWidth - 2, 0);
                }
                else
                {
                    canvas.Translate(-20, 0);
                }
            }
            else
            {
                canvas.Translate(this.Left, 0);
            }

            canvas.DrawRoundRect(0, 0, panelWidth, panelHeight, 10, 10, Brushes.PanelBackground);

            if (this.Title is { Length: > 0 })
            {
                canvas.Save();

                if (this.Position != PanelPosition.Right)
                {
                    canvas.Save();
                    canvas.RotateDegrees(180, panelWidth / 2, 10 + ((this.TitleWidth + 10) / 2));
                }

                using (var _ = canvas.Scope())
                {
                    canvas.ClipRect(new Rectangle(0, 10, TitleAreaWidth / 2, this.TitleWidth + 20), true, true);
                    canvas.DrawRoundRect(-TitleAreaWidth, 10, TitleAreaWidth + 3, this.TitleWidth + 10, 5, 5, Brushes.PanelBackground);
                    canvas.DrawRoundRect(-TitleAreaWidth, 10, TitleAreaWidth + 3, this.TitleWidth + 10, 5, 5, Brushes.PanelBorder);
                }

                using (var _ = canvas.Scope())
                {
                    //canvas.Translate(0, yPos);
                    canvas.RotateDegrees(270);
                    canvas.DrawText(this.Title, -15 - this.TitleWidth, -5, Brushes.Label);
                }

                if (this.Position == PanelPosition.Right)
                {
                    canvas.ClipRect(new Rectangle(-2, 10, TitleAreaWidth / 2, this.TitleWidth + 20), true, true);
                }
                else
                {
                    canvas.Restore();
                    canvas.ClipRect(new Rectangle(panelWidth - 3, 10, (panelWidth - 3) + TitleAreaWidth / 2, this.TitleWidth + 20), true, true);
                }
            }

            canvas.DrawRoundRect(0, 0, panelWidth, panelHeight, 10, 10, Brushes.PanelBorder);

            if (this.TitleWidth > 0)
            {
                canvas.Restore();
            }

            if (this.Position == PanelPosition.Left)
            {
                canvas.Translate(25, this.TopPadding);
            }
            else
            {
                canvas.Translate(10, this.TopPadding);
            }
        }
    }
}
