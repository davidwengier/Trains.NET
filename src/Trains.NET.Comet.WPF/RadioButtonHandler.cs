using Comet;
using Comet.WPF.Handlers;

using WPFToggleButton = System.Windows.Controls.Primitives.ToggleButton;

namespace Trains.NET.Comet.WPF
{
    internal class RadioButtonHandler : AbstractHandler<RadioButton, WPFToggleButton>
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

        protected override void DisposeView(WPFToggleButton nativeView)
        {
            nativeView.Click -= HandleClick;
        }

        private void HandleClick(object sender, System.Windows.RoutedEventArgs e)
        {
            this.VirtualView?.OnClick?.Invoke();
        }

        protected override WPFToggleButton CreateView()
        {
            var toggleButton = new WPFToggleButton();
            toggleButton.Click += HandleClick;
            return toggleButton;
        }

        public static void MapLabelProperty(IViewHandler viewHandler, RadioButton virtualRadioButton)
        {
            var nativeRadioButton = (WPFToggleButton)viewHandler.NativeView;
            nativeRadioButton.Content = virtualRadioButton.Label.CurrentValue;
        }

        public static void MapSelectedProperty(IViewHandler viewHandler, RadioButton virtualRadioButton)
        {
            var nativeRadioButton = (WPFToggleButton)viewHandler.NativeView;
            nativeRadioButton.IsChecked = virtualRadioButton.Selected;
        }
    }
}
