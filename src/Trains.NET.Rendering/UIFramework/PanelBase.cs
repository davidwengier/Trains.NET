using System;

namespace Trains.NET.Rendering.UI
{
    public abstract class PanelBase : IScreen, IInteractionHandler
    {
        private const int CloseButtonWidth = 24;
        private const int CloseButtonSize = 16;
        private const int TitleAreaWidth = 20;

        private bool _mouseHasBeenWithin;
        private bool _collapsed = true;
        private int _titleWidth;
        private bool _visible = true;

        public virtual bool PreHandleNextClick { get; set; }

        protected virtual bool AutoClose { get; }
        protected virtual bool CanClose { get; }
        protected virtual bool IsCollapsable { get; }
        protected virtual string? Title { get; }
        protected virtual int Top { get; set; }
        protected virtual int TopPadding { get; } = 15;
        protected virtual int BottomPadding { get; } = 15;
        protected virtual int CornerRadius { get; } = 10;
        protected virtual PanelPosition Position { get; } = PanelPosition.Left;
        protected virtual int Left { get; set; }

        protected virtual int InnerWidth { get; set; } = 100;
        protected virtual int InnerHeight { get; set; } = 100;

        protected virtual PaintBrush PanelBorderBrush => Brushes.PanelBorder;

        public bool Visible
        {

            get => _visible;
            set
            {
                _visible = value;
                this.PreHandleNextClick = value && this.AutoClose;
                _mouseHasBeenWithin = false;
            }
        }

        public event EventHandler? Changed;

        private int GetLeft(int width)
            => this.Left < 0
                ? width - (-1 * this.Left)
                : this.Left;

        public bool HandlePointerAction(int x, int y, int width, int height, PointerAction action)
        {
            if (!this.Visible)
            {
                this.PreHandleNextClick = false;
                return false;
            }

            if (action is not (PointerAction.Move or PointerAction.Click))
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
                x -= GetLeft(width);
            }

            if (x >= -TitleAreaWidth && x <= 0 && y >= 10 && y <= 10 + _titleWidth)
            {
                if (IsCollapsed())
                {
                    _collapsed = !_collapsed;
                    OnChanged();
                }
                else if (action == PointerAction.Click && y <= 10 + CloseButtonWidth && this.CanClose)
                {
                    this.Visible = false;
                    Close();
                }
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

                if (this.AutoClose)
                {
                    _mouseHasBeenWithin = true;
                }

                return HandlePointerAction(x, y, action);
            }
            else if (this.IsCollapsable && !_collapsed)
            {
                _collapsed = true;
                OnChanged();
            }

            if ((_mouseHasBeenWithin || action is PointerAction.Click) && this.AutoClose)
            {
                this.Visible = false;
            }

            return false;
        }

        protected virtual void Close()
        {
        }

        public void Render(ICanvas canvas, int width, int height)
        {
            if (!this.Visible)
            {
                return;
            }

            using (canvas.Scope())
            {
                PreRender(canvas);
            }

            if (this.Position == PanelPosition.Floating)
            {
                this.Left = Math.Max(10, this.Left);
                this.Left = Math.Min(width - GetPanelWidth() - 10, this.Left);
            }

            this.Top = Math.Max(10, this.Top);
            this.Top = Math.Min(height - GetPanelHeight() - 10, this.Top);

            canvas.Translate(0, this.Top);

            _titleWidth = 0;
            if (this.Title is { Length: > 0 })
            {
                _titleWidth = (int)canvas.MeasureText(this.Title, Brushes.Label);
            }
            if (this.CanClose)
            {
                _titleWidth += CloseButtonWidth;
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
                canvas.Translate(GetLeft(width), 0);
            }

            canvas.DrawRoundRect(0, 0, panelWidth, panelHeight, this.CornerRadius, this.CornerRadius, Brushes.PanelBackground);

            if (this.Title is { Length: > 0 } || this.CanClose)
            {
                canvas.Save();

                if (this.Position == PanelPosition.Left)
                {
                    canvas.Save();
                    canvas.RotateDegrees(180, panelWidth / 2, 10 + ((_titleWidth + 10) / 2));
                }

                using (canvas.Scope())
                {
                    canvas.ClipRect(new Rectangle(0, 10, TitleAreaWidth / 2, _titleWidth + 20), true, true);
                    canvas.DrawRoundRect(-TitleAreaWidth, 10, TitleAreaWidth + 3, _titleWidth + 10, 5, 5, Brushes.PanelBackground);
                    canvas.DrawRoundRect(-TitleAreaWidth, 10, TitleAreaWidth + 3, _titleWidth + 10, 5, 5, Brushes.PanelBorder);
                }

                var title = this.Title ?? "";
                using (canvas.Scope())
                {
                    canvas.RotateDegrees(270);
                    canvas.DrawText(title, -15 - _titleWidth, -5, Brushes.Label);
                }
                if (this.CanClose)
                {
                    canvas.DrawPicture(Picture.Cross, -TitleAreaWidth + 5, 10 + (CloseButtonWidth - CloseButtonSize) / 2, CloseButtonSize);
                }

                if (this.Position != PanelPosition.Left)
                {
                    canvas.ClipRect(new Rectangle(-2, 10, TitleAreaWidth / 2, _titleWidth + 20), true, true);
                }
                else
                {
                    canvas.Restore();
                    canvas.ClipRect(new Rectangle(panelWidth - 3, 10, (panelWidth - 3) + TitleAreaWidth / 2, _titleWidth + 20), true, true);
                }
            }

            canvas.DrawRoundRect(0, 0, panelWidth, panelHeight, this.CornerRadius, this.CornerRadius, this.PanelBorderBrush);

            if (_titleWidth > 0)
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

            if (IsCollapsed())
            {
                return;
            }

            Render(canvas);
        }

        protected virtual bool HandlePointerAction(int x, int y, PointerAction action)
        {
            return true;
        }

        protected virtual void PreRender(ICanvas canvas)
        {
        }

        protected virtual void Render(ICanvas canvas)
        {
        }

        protected void OnChanged()
        {
            Changed?.Invoke(this, EventArgs.Empty);
        }

        protected bool IsCollapsed()
            => CanCollapse() && _collapsed;

        private bool CanCollapse()
            => this.IsCollapsable && this.Title is { Length: > 0 } && this.Position != PanelPosition.Floating;

        protected int GetPanelHeight()
            => Math.Max(_titleWidth, this.InnerHeight) + this.TopPadding + this.BottomPadding;

        protected int GetPanelWidth()
            => this.InnerWidth + 20;
    }
}
