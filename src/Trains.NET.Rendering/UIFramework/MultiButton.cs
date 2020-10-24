namespace Trains.NET.Rendering.UI
{
    public class MultiButton : Button
    {
        private readonly int _buttonSize;
        private readonly Button[] _buttons;

        public MultiButton(int buttonSize, params Button[] buttons)
        {
            _buttonSize = buttonSize;
            _buttons = buttons;

            this.Height = _buttonSize;
            this.Width = _buttonSize * _buttons.Length;
        }

        public override int GetMinimumWidth(ICanvas canvas) => _buttonSize * _buttons.Length;

        public override bool HandleMouseAction(int x, int y, PointerAction action)
        {
            if (action is Rendering.PointerAction.Click or Rendering.PointerAction.Move)
            {
                foreach (var button in _buttons)
                {
                    if (button.HandleMouseAction(x, y, action))
                    {
                        return true;
                    }

                    x -= button.Width;
                }
            }
            return false;
        }

        public override void Render(ICanvas canvas)
        {
            using (canvas.Scope())
            {
                foreach (var button in _buttons)
                {
                    button.Width = _buttonSize;
                    button.Height = _buttonSize;

                    button.Render(canvas);
                    canvas.Translate(_buttonSize, 0);
                }
            }
        }
    }
}
