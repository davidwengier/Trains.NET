using System.Windows;
using Comet;
using Comet.WPF;
using Comet.WPF.Handlers;

namespace Trains.NET.Comet.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            global::Comet.Skia.UI.Init();

            Registrar.Handlers.Register<HStack, HStackHandler>();
            Registrar.Handlers.Register<VStack, VStackHandler>();

            MainFrame.NavigationUIVisibility = System.Windows.Navigation.NavigationUIVisibility.Hidden;
            MainFrame.NavigationService.Navigate(new CometPage(MainFrame, new MainPage()));
        }
    }
}
