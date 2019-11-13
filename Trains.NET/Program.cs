using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using Trains.NET.Engine;
using Trains.NET.Rendering;

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

            using var form = CreateFromDiscovery();

            Application.Run(form);
        }

        private static MainForm CreateFormManual()
        {
            var gameBoard = new GameBoard();
            return new MainForm(
                new Game(
                    gameBoard,
                    new List<IBoardRenderer>
                    {
                        new GridRenderer(),
                        new TrackLayoutRenderer(
                            gameBoard,
                            new TrackRenderer())
                    }));
        }

        private static MainForm CreateFromContainer()
        {
            var col = new ServiceCollection();

            col.AddSingleton<IGame, Game>();
            col.AddSingleton<IGameBoard, GameBoard>();
            col.AddSingleton<ITrackRenderer, TrackRenderer>();
            col.AddSingleton<IBoardRenderer, GridRenderer>();
            col.AddSingleton<IBoardRenderer, TrackLayoutRenderer>();

            ServiceProvider serviceProvider = col.BuildServiceProvider();

            return new MainForm(serviceProvider.GetService<IGame>());
        }

        private static MainForm CreateFromDiscovery()
        {
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

            ServiceProvider serviceProvider = col.BuildServiceProvider();

            return new MainForm(serviceProvider.GetService<IGame>());
        }

        private static IEnumerable<Assembly> GetAssemblies()
        {
            yield return typeof(IGameBoard).Assembly;
            yield return typeof(IGame).Assembly;
        }
    }
}
