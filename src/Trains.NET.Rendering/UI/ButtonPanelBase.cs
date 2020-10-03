using System;
using System.Collections.Generic;
using System.Linq;

namespace Trains.NET.Rendering.UI
{
    public abstract partial class ButtonPanelBase : PanelBase
    {
        private const int TextXPadding = 10;
        private const int ButtonGap = 10;
        private const int ButtonLeft = 5;
        private float _buttonWidth = 60;

        private Button? _hoverButton;

        public override bool HandleInteraction(int x, int y, int width, int height, MouseAction action)
        {
            if (action is MouseAction.Click or MouseAction.Move)
            {
                var buttons = GetButtons().ToList();

                int yPos = this.Top;

                var panelWidth = ButtonLeft + (int)_buttonWidth + 20;
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

                if (this.Title is { Length: > 0 })
                {
                    if (x >= -TitleAreaWidth && x <= 0 && y >= yPos + 10 && y <= yPos + 10 + base.TitleWidth)
                    {
                        base.Collapsed = !base.Collapsed;
                        OnChanged();
                        return true;
                    }
                }

                if (IsCollapsed())
                {
                    return false;
                }

                var buttonPanelHeight = buttons.Sum(b => b.Height + ButtonGap) - ButtonGap;
                if (x is >= 0 && x <= panelWidth && y >= yPos && y <= yPos + buttonPanelHeight + 20)
                {
                    if (this.Position == PanelPosition.Left)
                    {
                        x -= 5;
                    }
                    else
                    {
                        x -= 10;
                    }

                    yPos += this.TopPadding;
                    foreach (Button button in GetButtons())
                    {
                        if (x is >= ButtonLeft && x <= ButtonLeft + _buttonWidth && y >= yPos && y <= yPos + button.Height)
                        {
                            if (action == MouseAction.Click)
                            {
                                button.Click();
                            }
                            else
                            {
                                _hoverButton = button;
                            }
                            OnChanged();
                            return true;
                        }

                        yPos += (int)button.Height + ButtonGap;
                    }
                    return true;
                }
                else if (this.IsCollapsable && !base.Collapsed)
                {
                    base.Collapsed = true;
                    OnChanged();
                }
                _hoverButton = null;
            }
            return false;
        }

        public override void Render(ICanvas canvas, int width, int height)
        {
            var buttons = GetButtons().ToList();

            _buttonWidth = 0;
            base.InnerHeight = 0;
            foreach (Button button in buttons)
            {
                _buttonWidth = Math.Max(_buttonWidth, button.GetMinimumWidth(canvas));
                base.InnerHeight += button.Height + ButtonGap;
            }
            _buttonWidth += TextXPadding * 2;

            base.InnerWidth = _buttonWidth + 10;
            base.InnerHeight = base.InnerHeight - ButtonGap;

            base.Render(canvas, width, height);

            canvas.Translate(ButtonLeft, 0);

            foreach (Button button in buttons)
            {
                button.Width = _buttonWidth;

                button.Render(canvas, button == _hoverButton);

                canvas.Translate(0, button.Height + ButtonGap);
            }
        }

        protected abstract IEnumerable<Button> GetButtons();
    }
}
