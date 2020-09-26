using System;
using System.Collections.Generic;
using System.Linq;

namespace Trains.NET.Rendering.UI
{
    public abstract class ButtonPanelBase : IScreen
    {
#pragma warning disable IDE1006 // Naming Styles
        protected record Button(string Label, object Item, Func<bool> IsActive, Action Click);
#pragma warning restore IDE1006 // Naming Styles

        private const int TextYPadding = 15;
        private const int TextXPadding = 10;
        private const int ButtonGap = 10;
        private const int ButtonLeft = 5;
        private const int ButtonHeight = 20;
        private float _buttonWidth = 60;
        private float _titleWidth;

        private Button? _hoverButton;

        protected virtual bool Collapsed { get; set; }
        protected virtual string? Title { get; }
        protected abstract int Top { get; }
        protected virtual int TopPadding { get; } = 15;
        protected virtual PanelSide Side { get; } = PanelSide.Left;

        public event EventHandler? Changed;

        public bool HandleInteraction(int x, int y, int width, int height, MouseAction action)
        {
            if (action is MouseAction.Click or MouseAction.Move)
            {
                var buttons = GetButtons().ToList();

                int yPos = this.Top;

                var panelWidth = ButtonLeft + (int)_buttonWidth + 20;
                if (this.Side == PanelSide.Right)
                {
                    if (this.Collapsed)
                    {
                        x -= width;
                    }
                    else
                    {
                        x -= (width - panelWidth);
                    }
                }
                else
                {
                    if (this.Collapsed)
                    {
                        x -= ButtonHeight;
                    }
                }

                if (this.Title is { Length: > 0 })
                {
                    if (x >= -ButtonHeight && x <= 0 && y >= yPos + 10 && y <= yPos + 10 + _titleWidth)
                    {
                        this.Collapsed = !this.Collapsed;
                        OnChanged();
                        return true;
                    }
                }

                if (this.Collapsed)
                {
                    return false;
                }

                if (x is >= 0 && x <= panelWidth && y >= yPos && y <= yPos + buttons.Count * (ButtonHeight + ButtonGap) + 20)
                {
                    if (this.Side == PanelSide.Right)
                    {
                        x -= 10;
                    }
                    else
                    {
                        x -= 5;
                    }

                    yPos += this.TopPadding;
                    foreach (Button button in GetButtons())
                    {
                        if (x is >= ButtonLeft && x <= ButtonLeft + _buttonWidth && y >= yPos && y <= yPos + ButtonHeight)
                        {
                            if (action == MouseAction.Click)
                            {
                                button.Click?.Invoke();
                            }
                            else
                            {
                                _hoverButton = button;
                            }
                            OnChanged();
                            return true;
                        }

                        yPos += ButtonHeight + ButtonGap;
                    }
                    return true;
                }
                else if (this.Title is { Length: > 0 } && !this.Collapsed)
                {
                    this.Collapsed = true;
                    OnChanged();
                }
                _hoverButton = null;
            }
            return false;
        }

        protected void OnChanged()
        {
            Changed?.Invoke(this, EventArgs.Empty);
        }

        public virtual void Render(ICanvas canvas, int width, int height)
        {
            int yPos = this.Top;

            _titleWidth = 0f;
            if (this.Title is { Length: > 0 })
            {
                _titleWidth = canvas.MeasureText(this.Title, Brushes.Label);
            }

            var buttons = GetButtons().ToList();
            _buttonWidth = _titleWidth;
            foreach (Button button in buttons)
            {
                _buttonWidth = Math.Max(_buttonWidth, canvas.MeasureText(button.Label, Brushes.Label));
            }
            _buttonWidth += TextXPadding;

            var panelHeight = Math.Max(_titleWidth + 20, buttons.Count * (ButtonGap + ButtonHeight) + 20);
            var panelWidth = 20 + ButtonLeft + _buttonWidth + 20;

            if (this.Side == PanelSide.Right)
            {
                if (this.Collapsed)
                {
                    canvas.Translate(width + 2, 0);
                }
                else
                {
                    canvas.Translate(width - panelWidth + 20, 0);
                }
            }
            else
            {
                if (this.Collapsed)
                {
                    canvas.Translate(-panelWidth - 2, 0);
                }
                else
                {
                    canvas.Translate(-20, 0);
                }
            }

            canvas.DrawRoundRect(0, yPos, panelWidth, panelHeight, 10, 10, Brushes.PanelBackground);

            if (this.Title is { Length: > 0 })
            {
                canvas.Save();

                if (this.Side == PanelSide.Left)
                {
                    canvas.Save();
                    canvas.RotateDegrees(180, panelWidth / 2, yPos + 10 + ((_titleWidth + 10) / 2));
                }

                using (var _ = canvas.Scope())
                {
                    canvas.ClipRect(new Rectangle(0, yPos + 10, ButtonHeight / 2, yPos + _titleWidth + 20), true, true);
                    canvas.DrawRoundRect(-ButtonHeight, yPos + 10, ButtonHeight + 3, _titleWidth + 10, 5, 5, Brushes.PanelBackground);
                    canvas.DrawRoundRect(-ButtonHeight, yPos + 10, ButtonHeight + 3, _titleWidth + 10, 5, 5, Brushes.PanelBorder);
                }

                using (var _ = canvas.Scope())
                {
                    canvas.Translate(0, yPos);
                    canvas.RotateDegrees(270);
                    canvas.DrawText(this.Title, -15 - _titleWidth, -5, Brushes.Label);
                }

                if (this.Side == PanelSide.Left)
                {
                    canvas.Restore();
                    canvas.ClipRect(new Rectangle(panelWidth - 3, yPos + 10, (panelWidth - 3) + ButtonHeight / 2, yPos + _titleWidth + 20), true, true);
                }
                else
                {
                    canvas.ClipRect(new Rectangle(-2, yPos + 10, ButtonHeight / 2, yPos + _titleWidth + 20), true, true);
                }
            }

            canvas.DrawRoundRect(0, yPos, panelWidth, panelHeight, 10, 10, Brushes.PanelBorder);

            if (_titleWidth > 0)
            {
                canvas.Restore();
            }

            yPos += this.TopPadding;

            if (this.Side == PanelSide.Right)
            {
                canvas.Translate(10, 0);
            }
            else
            {
                canvas.Translate(25, 0);
            }

            foreach (Button button in buttons)
            {
                var isActive = button.IsActive?.Invoke() ?? false;
                PaintBrush brush = isActive ? Brushes.ButtonActiveBackground
                    : Brushes.ButtonBackground;
                canvas.DrawRect(ButtonLeft, yPos, _buttonWidth, ButtonHeight, brush);
                if (button == _hoverButton)
                {
                    canvas.DrawRect(ButtonLeft, yPos, _buttonWidth, ButtonHeight, Brushes.ButtonHoverBackground);
                }

                float textWidth = canvas.MeasureText(button.Label, Brushes.Label);

                canvas.DrawText(button.Label, ButtonLeft + ((_buttonWidth - textWidth) / 2), yPos + TextYPadding, Brushes.Label);
                yPos += ButtonHeight + ButtonGap;
            }
        }

        protected abstract IEnumerable<Button> GetButtons();
    }
}
