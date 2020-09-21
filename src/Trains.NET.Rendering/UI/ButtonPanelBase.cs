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

        private Button? _hoverButton;

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
                    x -= (width - panelWidth);
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

            var buttons = GetButtons().ToList();
            _buttonWidth = 0;
            foreach (Button button in buttons)
            {
                _buttonWidth = Math.Max(_buttonWidth, canvas.MeasureText(button.Label, Brushes.Label));
            }
            _buttonWidth += TextXPadding;

            var panelHeight = buttons.Count * (ButtonGap + ButtonHeight) + 20;
            var panelWidth = 20 + ButtonLeft + _buttonWidth + 20;

            if (this.Side == PanelSide.Right)
            {
                canvas.Translate(width - panelWidth + 20, 0);
            }
            else
            {
                canvas.Translate(-20, 0);
            }

            canvas.DrawRoundRect(0, yPos, panelWidth, panelHeight, 10, 10, Brushes.PanelBorder);
            canvas.DrawRoundRect(0, yPos, panelWidth, panelHeight, 10, 10, Brushes.PanelBackground);

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
                PaintBrush brush = button == _hoverButton ? Brushes.ButtonHoverBackground
                            : button.IsActive?.Invoke() ?? false ? Brushes.ButtonActiveBackground
                            : Brushes.ButtonBackground;
                canvas.DrawRect(ButtonLeft, yPos, _buttonWidth, ButtonHeight, brush);

                float textWidth = canvas.MeasureText(button.Label, Brushes.Label);

                canvas.DrawText(button.Label, ButtonLeft + ((_buttonWidth - textWidth) / 2), yPos + TextYPadding, Brushes.Label);
                yPos += ButtonHeight + ButtonGap;
            }
        }

        protected abstract IEnumerable<Button> GetButtons();
    }
}
