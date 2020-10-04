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
        private int _buttonWidth = 60;

        private Button? _hoverButton;

        protected abstract IEnumerable<Button> GetButtons();

        protected override bool HandleMouseAction(int x, int y, MouseAction action)
        {
            if (action is MouseAction.Click or MouseAction.Move)
            {
                x -= ButtonLeft;
                foreach (Button button in GetButtons())
                {
                    if (x is >= 0 && x <= _buttonWidth && y >= 0 && y <= button.Height)
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

                    y -= button.Height + ButtonGap;
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
    }
}
