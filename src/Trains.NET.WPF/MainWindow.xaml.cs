using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using Comet;
using Comet.WPF;
using Microsoft.Extensions.DependencyInjection;
using Trains.NET.Comet;
using Trains.NET.Engine;

namespace Trains.NET.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            ServiceProvider serviceProvider = BuildServiceProvider();

            InitializeComponent();

            global::Comet.WPF.UI.Init();
            global::Comet.Skia.UI.Init();

            Registrar.Handlers.Register<RadioButton, RadioButtonHandler>();
            Registrar.Handlers.Register<ToggleButton, ToggleButtonHandler>();

            MainFrame.NavigationUIVisibility = System.Windows.Navigation.NavigationUIVisibility.Hidden;
            MainFrame.NavigationService.Navigate(new CometPage(MainFrame, serviceProvider.GetService<MainPage>()));
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show("An error has occurred:\n\n" + e.ExceptionObject.ToString());
        }

        private ServiceProvider BuildServiceProvider()
        {
            var col = new ServiceCollection();
            foreach (Assembly a in GetAssemblies())
            {
                foreach (Type t in a.GetTypes().Where(x=>!x.Name.EndsWith("Stat")))
                {
                    if (t.IsInterface)
                    {
                        Type orderedListOfT = typeof(OrderedList<>).MakeGenericType(t);
                        col.AddSingleton(orderedListOfT, sp => Activator.CreateInstance(orderedListOfT, sp.GetServices(t)));
                    }
                    else
                    {
                        foreach (Type inter in t.GetInterfaces())
                        {
                            if (inter.Namespace?.StartsWith("Trains.NET", StringComparison.OrdinalIgnoreCase) == true)
                            {
                                col.AddSingleton(inter, t);
                            }
                        }
                    }
                }
            }

            col.AddSingleton<MainPage, MainPage>();

            return col.BuildServiceProvider();

            static IEnumerable<Assembly> GetAssemblies()
            {
                yield return typeof(Trains.NET.Engine.IGameBoard).Assembly;
                yield return typeof(Trains.NET.Rendering.IGame).Assembly;
                yield return typeof(Trains.NET.Rendering.Skia.SKCanvasWrapper).Assembly;
                yield return typeof(MainWindow).Assembly;
                yield return typeof(MainPage).Assembly;
            }
        }
    }
}
