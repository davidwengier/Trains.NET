using System;
using Comet;

namespace Trains.NET.Comet
{
    public class ToggleButton : View
    {
        private Binding<bool>? _selected;
        private Binding<string>? _label;

        public ToggleButton(
            Binding<string>? label = null,
            Binding<bool>? selected = null,
            Action? onClick = null)
        {
            this.Label = label;
            this.Selected = selected;
            this.OnClick = onClick;
        }

        public Binding<string>? Label
        {
            get => _label;
            private set => this.SetBindingValue(ref _label, value);
        }

        public Binding<bool>? Selected
        {
            get => _selected;
            private set => this.SetBindingValue(ref _selected, value);
        }

        public Action? OnClick { get; private set; }
    }
}
