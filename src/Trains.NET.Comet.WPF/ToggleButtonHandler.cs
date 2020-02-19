using Comet;
using Comet.WPF.Handlers;

using WPFToggleButton = System.Windows.Controls.Primitives.ToggleButton;

namespace Trains.NET.Comet.WPF
{
    internal class ToggleButtonHandler : AbstractHandler<ToggleButton, WPFToggleButton>
    {
        public static readonly PropertyMapper<ToggleButton> Mapper = new PropertyMapper<ToggleButton>()
        {
            [nameof(ToggleButton.Label)] = MapLabelProperty,
            [nameof(ToggleButton.Selected)] = MapSelectedProperty
        };

        public ToggleButtonHandler()
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

        public static void MapLabelProperty(IViewHandler viewHandler, ToggleButton virtualToggleButton)
        {
            var nativeToggleButton = (WPFToggleButton)viewHandler.NativeView;
            nativeToggleButton.Content = virtualToggleButton.Label?.CurrentValue;
        }

        public static void MapSelectedProperty(IViewHandler viewHandler, ToggleButton virtualToggleButton)
        {
            var nativeToggleButton = (WPFToggleButton)viewHandler.NativeView;
            nativeToggleButton.IsChecked = virtualToggleButton.Selected;
        }
    }
}
