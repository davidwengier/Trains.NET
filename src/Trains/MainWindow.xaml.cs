using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows;
using Comet;
using Comet.WPF;
using Microsoft.Extensions.DependencyInjection;
using Trains.Handlers;
using Trains.NET.Comet;
using Trains.NET.Engine;
using Trains.Storage;

namespace Trains
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string _windowSizeFileName = FileSystemStorage.GetFilePath("WindowSize.txt");

        public MainWindow()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            ServiceProvider serviceProvider = BuildServiceProvider();

            InitializeComponent();

            global::Comet.WPF.UI.Init();
            global::Comet.Skia.UI.Init();

            Registrar.Handlers.Register<RadioButton, RadioButtonHandler>();
            Registrar.Handlers.Register<ToggleButton, ToggleButtonHandler>();

            if (File.Exists(_windowSizeFileName))
            {
                string sizeString = File.ReadAllText(_windowSizeFileName);
                string[] bits = sizeString.Split(',');
                if (bits.Length == 2)
                {
                    if (double.TryParse(bits[0], out double width) && double.TryParse(bits[1], out double height))
                    {
                        this.Width = width;
                        this.Height = height;
                    }
                }
            }

            MainFrame.NavigationUIVisibility = System.Windows.Navigation.NavigationUIVisibility.Hidden;
            var page = new CometPage(MainFrame, serviceProvider.GetService<MainPage>());
            MainFrame.Content = page;

            this.Title = page.View.GetTitle();

            SizeChanged += MainWindow_SizeChanged;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (MainFrame.Content is CometPage cometPage && cometPage.View is MainPage mainPage)
            {
                mainPage.Save();
                mainPage.Dispose();
            }
            File.WriteAllText(_windowSizeFileName, $"{this.Width},{this.Height}");
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (MainFrame.Content is CometPage cometPage && cometPage.View is MainPage mainPage)
            {
                mainPage.Redraw(e.NewSize);
            }
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show("An error has occurred:\n\n" + e.ExceptionObject.ToString());
        }

        private static ServiceProvider BuildServiceProvider()
        {
            var col = new ServiceCollection();
            foreach (Assembly a in GetAssemblies())
            {
                foreach (Type t in a.GetTypes())
                {
                    if (t.IsInterface)
                    {
                        WireUpOrderedList(col, t);
                        WireUpFunc(col, t);
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

            static void WireUpOrderedList(ServiceCollection col, Type t)
            {
                Type orderedListOfT = typeof(OrderedList<>).MakeGenericType(t);
                col.AddSingleton(orderedListOfT, sp => Activator.CreateInstance(orderedListOfT, sp.GetServices(t)));
            }

            static void WireUpFunc(ServiceCollection col, Type t)
            {
                Type orderedListOfT = typeof(OrderedList<>).MakeGenericType(t);
                Type factoryType = typeof(Factory<>).MakeGenericType(t);
                col.AddSingleton(factoryType, sp => Activator.CreateInstance(factoryType, sp.GetService(orderedListOfT)));
            }
        }
    }
}
