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
        private const int ButtonLeft = 15;
        private const int ButtonHeight = 20;
        private float _buttonWidth = 60;

        private Button? _hoverButton;

        protected abstract int Top { get; }
        protected virtual int TopPadding { get; } = 15;

        public event EventHandler? Changed;

        public bool HandleInteraction(int x, int y, int width, int height, MouseAction action)
        {
            if (action is MouseAction.Click or MouseAction.Move)
            {
                var buttons = GetButtons().ToList();

                int yPos = this.Top;

                if (x is >= 0 && x <= ButtonLeft + _buttonWidth + 20 && y >= yPos && y <= yPos + buttons.Count * (ButtonHeight + ButtonGap) + 20)
                {
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

            canvas.DrawRoundRect(-50, yPos, 50 + ButtonLeft + _buttonWidth + 20, buttons.Count * (ButtonGap + ButtonHeight) + 20, 10, 10, Brushes.PanelBorder);
            canvas.DrawRoundRect(-50, yPos, 50 + ButtonLeft + _buttonWidth + 20, buttons.Count * (ButtonGap + ButtonHeight) + 20, 10, 10, Brushes.PanelBackground);

            yPos += this.TopPadding;

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
