namespace Trains.NET.Rendering.UI;

public class MultiButton : ButtonBase
{
    private readonly int _buttonSize;
    private readonly ButtonBase[] _buttons;

    public MultiButton(int buttonSize, params ButtonBase[] buttons)
    {
        _buttonSize = buttonSize;
        _buttons = buttons;

        Height = _buttonSize;
        Width = _buttonSize * _buttons.Length;
    }

    public override int GetMinimumWidth(ICanvas canvas) => _buttonSize * _buttons.Length;

    public override bool HandleMouseAction(int x, int y, PointerAction action)
    {
        if (action is Rendering.PointerAction.Click or Rendering.PointerAction.Move)
        {
            var handled = false;
            foreach (var button in _buttons)
            {
                if (button.HandleMouseAction(x, y, action))
                {
                    // Moves must reach every child so previously hovered buttons are cleared.
                    if (action != Rendering.PointerAction.Move)
                    {
                        return true;
                    }

                    handled = true;
                }

                x -= button.Width;
            }

            return handled;
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

    protected override void RenderButtonLabel(ICanvas canvas)
    {
        throw new InvalidOperationException("This should never be hit");
    }
}
