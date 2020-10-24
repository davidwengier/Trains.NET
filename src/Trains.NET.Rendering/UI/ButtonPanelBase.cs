using System;
using System.Collections.Generic;
using System.Linq;

namespace Trains.NET.Rendering.UI
{
    public abstract partial class ButtonPanelBase : PanelBase
    {
        private const int ButtonGap = 10;
        private const int ButtonLeft = 5;
        private int _buttonWidth = 60;

        protected abstract IEnumerable<Button> GetButtons();

        protected override bool HandlePointerAction(int x, int y, PointerAction action)
        {
            if (action is Rendering.PointerAction.Click or Rendering.PointerAction.Move)
            {
                x -= ButtonLeft;
                foreach (var button in GetButtons())
                {
                    if (button.HandleMouseAction(x, y, action))
                    {
                        OnChanged();
                        return true;
                    }

                    y -= button.Height + ButtonGap;
                }
            }
            return true;
        }

        protected override void Render(ICanvas canvas)
        {
            canvas.Translate(ButtonLeft, 0);

            foreach (Button button in GetButtons().ToArray())
            {
                button.Width = _buttonWidth;

                using (canvas.Scope())
                {
                    button.Render(canvas);
                }

                canvas.Translate(0, button.Height + ButtonGap);
            }
        }

        protected override void CalculateSize(ICanvas canvas)
        {
            _buttonWidth = 0;
            base.InnerHeight = 0;
            foreach (Button button in GetButtons().ToArray())
            {
                _buttonWidth = Math.Max(_buttonWidth, button.GetMinimumWidth(canvas));
                base.InnerHeight += button.Height + ButtonGap;
            }

            base.InnerWidth = _buttonWidth + 10;
            base.InnerHeight = base.InnerHeight - ButtonGap;
        }
    }
}
