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

        private const int TextPadding = 15;
        private const int ButtonGap = 10;
        private const int ButtonLeft = 15;
        private const int ButtonHeight = 20;
        private const int ButtonWidth = 60;

        private Button? _hoverButton;

        protected abstract int Top { get; }

        public event EventHandler? Changed;

        public bool HandleInteraction(int x, int y, int width, int height, MouseAction action)
        {
            if (action is MouseAction.Click or MouseAction.Move)
            {
                var buttons = GetButtons().ToList();

                int yPos = ButtonHeight * 3;

                if (x is >= 0 and <= ButtonLeft + ButtonWidth + 20 && y >= yPos && y <= yPos + buttons.Count * (ButtonHeight + ButtonGap) + 20)
                {
                    yPos += 20;
                    foreach (Button button in GetButtons())
                    {
                        if (x is >= ButtonLeft and <= ButtonLeft + ButtonWidth && y >= yPos && y <= yPos + ButtonHeight)
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

        public void Render(ICanvas canvas, int width, int height)
        {
            int yPos = this.Top;

            var buttons = GetButtons().ToList();

            canvas.DrawRoundRect(-50, yPos, 50 + ButtonLeft + ButtonWidth + 20, buttons.Count * (ButtonGap + ButtonHeight) + 20, 10, 10, Brushes.PanelBorder);
            canvas.DrawRoundRect(-50, yPos, 50 + ButtonLeft + ButtonWidth + 20, buttons.Count * (ButtonGap + ButtonHeight) + 20, 10, 10, Brushes.PanelBackground);

            yPos += 20;

            foreach (Button button in buttons)
            {
                PaintBrush brush = button == _hoverButton ? Brushes.ButtonHoverBackground
                            : button.IsActive?.Invoke() ?? false ? Brushes.ButtonActiveBackground
                            : Brushes.ButtonBackground;
                canvas.DrawRect(ButtonLeft, yPos, ButtonWidth, ButtonHeight, brush);

                float textWidth = canvas.MeasureText(button.Label, Brushes.Label);

                canvas.DrawText(button.Label, ButtonLeft + ((ButtonWidth - textWidth) / 2), yPos + TextPadding, Brushes.Label);
                yPos += ButtonHeight + ButtonGap;
            }
        }

        protected abstract IEnumerable<Button> GetButtons();
    }
}
