using System;

namespace Trains.NET.Rendering.UI
{
    public abstract class PanelBase : IScreen
    {
        protected const int TitleAreaWidth = 20;
        public event EventHandler? Changed;

        protected bool Collapsed { get; set; } = true;

        protected virtual bool IsCollapsable { get; }
        protected virtual string? Title { get; }
        protected abstract int Top { get; }
        protected virtual int TopPadding { get; } = 15;
        protected virtual int BottomPadding { get; } = 15;
        protected virtual PanelPosition Position { get; } = PanelPosition.Left;
        protected virtual int Left { get; }
        protected int TitleWidth { get; set; }
        protected int InnerWidth { get; set; } = 100;
        protected int InnerHeight { get; set; } = 100;

        protected void OnChanged()
        {
            Changed?.Invoke(this, EventArgs.Empty);
        }

        protected bool IsCollapsed()
            => CanCollapse() && this.Collapsed;

        private bool CanCollapse()
            => this.IsCollapsable && this.Title is { Length: > 0 } && this.Position != PanelPosition.Floating;

        public bool HandleInteraction(int x, int y, int width, int height, MouseAction action)
        {
            if (action is MouseAction.Drag or MouseAction.Release)
            {
                return false;
            }

            var panelHeight = GetPanelHeight();
            var panelWidth = GetPanelWidth();

            y -= this.Top;

            if (this.Position == PanelPosition.Right)
            {
                if (IsCollapsed())
                {
                    x -= width;
                }
                else
                {
                    x -= (width - panelWidth);
                }
            }
            else if (this.Position == PanelPosition.Left)
            {
                if (IsCollapsed())
                {
                    x -= TitleAreaWidth;
                }
            }
            else
            {
                x -= this.Left;
            }

            if (IsCollapsed() && x >= -TitleAreaWidth && x <= 0 && y >= 10 && y <= 10 + this.TitleWidth)
            {
                this.Collapsed = !this.Collapsed;
                OnChanged();
                return true;
            }
            else if (!IsCollapsed() && x is >= 0 && x <= panelWidth && y >= 0 && y <= panelHeight)
            {
                if (this.Position == PanelPosition.Left)
                {
                    x -= 5;
                }
                else
                {
                    x -= 10;
                }
                y -= this.TopPadding;

                return HandleMouseAction(x, y, action);
            }
            else if (this.IsCollapsable && !this.Collapsed)
            {
                this.Collapsed = true;
                OnChanged();
            }

            return false;
        }

        protected virtual bool HandleMouseAction(int x, int y, MouseAction action)
        {
            return true;
        }

        public virtual void Render(ICanvas canvas, int width, int height)
        {
            canvas.Translate(0, this.Top);

            this.TitleWidth = 0;
            if (this.Title is { Length: > 0 })
            {
                this.TitleWidth = (int)canvas.MeasureText(this.Title, Brushes.Label);
            }

            var panelHeight = GetPanelHeight();
            var panelWidth = GetPanelWidth();
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

        private int GetPanelHeight()
            => Math.Max(this.TitleWidth, this.InnerHeight) + this.TopPadding + this.BottomPadding;

        private int GetPanelWidth()
            => this.InnerWidth + 20;
    }
}
