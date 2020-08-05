using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using Comet;
using Comet.WPF;
using Microsoft.Extensions.DependencyInjection;
using Trains.Handlers;
using Trains.NET.Comet;
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

            ServiceProvider serviceProvider = Services.GetServiceProvider();

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
            MainPage mainPage = Trains.NET.Comet.Services.GetEntryPoint();

            if (mainPage == null)
            {
                mainPage = serviceProvider.GetService<MainPage>();
            }

            var page = new CometPage(MainFrame, mainPage);
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
    }
}
