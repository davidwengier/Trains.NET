using System.Windows.Controls.Primitives;
using Comet;
using Comet.WPF.Handlers;

namespace Trains.NET.WPF
{
    internal class RadioButtonHandler : AbstractHandler<RadioButton, ToggleButton>
    {
        public static readonly PropertyMapper<RadioButton> Mapper = new PropertyMapper<RadioButton>()
        {
            [nameof(RadioButton.Label)] = MapLabelProperty,
            [nameof(RadioButton.Selected)] = MapSelectedProperty
        };

        public RadioButtonHandler()
            : base(Mapper)
        {
        }

        protected override void DisposeView(ToggleButton nativeView)
        {
            nativeView.Click -= HandleClick;
        }

        private void HandleClick(object sender, System.Windows.RoutedEventArgs e)
        {
            this.VirtualView?.OnClick?.Invoke();
        }

        protected override ToggleButton CreateView()
        {
            var toggleButton = new ToggleButton();
            toggleButton.Click += HandleClick;
            return toggleButton;
        }

        public static void MapLabelProperty(IViewHandler viewHandler, RadioButton virtualRadioButton)
        {
            var nativeRadioButton = (ToggleButton)viewHandler.NativeView;
            nativeRadioButton.Content = virtualRadioButton.Label.CurrentValue;
        }

        public static void MapSelectedProperty(IViewHandler viewHandler, RadioButton virtualRadioButton)
        {
            var nativeRadioButton = (ToggleButton)viewHandler.NativeView;
            nativeRadioButton.IsChecked = virtualRadioButton.Selected;
        }
    }
}
