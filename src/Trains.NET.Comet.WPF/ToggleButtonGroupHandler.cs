using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls.Primitives;
using Comet.WPF.Handlers;

namespace Trains.NET.Comet.WPF
{
    internal class ToggleButtonGroupHandler : AbstractLayoutHandler
    {
        private readonly List<ToggleButton> _controls = new List<ToggleButton>();

        public ToggleButtonGroupHandler()
        {
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var result = base.ArrangeOverride(finalSize);

            List<ToggleButton> seen = new List<ToggleButton>();
            foreach (var control in this.Children)
            {
                if (control is ToggleButton btn)
                {
                    seen.Add(btn);
                    if (!_controls.Contains(btn))
                    {
                        _controls.Add(btn);
                        btn.Checked += HandleButtonChecked;
                    }
                }
            }

            foreach (var btn in _controls.ToArray())
            {
                if (!seen.Contains(btn))
                {
                    btn.Checked -= HandleButtonChecked;
                    _controls.Remove(btn);
                }
            }

            return result;
        }

        private void HandleButtonChecked(object sender, RoutedEventArgs e)
        {
            foreach (var btn in _controls)
            {
                if (btn != sender)
                {
                    btn.IsChecked = false;
                }
            }
        }
    }
}
