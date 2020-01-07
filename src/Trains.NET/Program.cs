using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;

namespace Trains.NET
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var col = new ServiceCollection();
            foreach (Assembly a in GetAssemblies())
            {
                foreach (Type t in a.GetTypes())
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

            col.AddSingleton<MainForm, MainForm>();

            ServiceProvider serviceProvider = col.BuildServiceProvider();

            using Form form = serviceProvider.GetService<MainForm>();

            Application.Run(form);

            static IEnumerable<Assembly> GetAssemblies()
            {
                yield return typeof(Trains.NET.Engine.IGameBoard).Assembly;
                yield return typeof(Trains.NET.Rendering.IGame).Assembly;
            }
        }
    }
}
