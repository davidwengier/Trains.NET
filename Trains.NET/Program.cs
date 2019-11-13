using System;
using System.Collections.Generic;
using System.Linq;
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

            using var form = CreateFromContainer();

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

            col.AddScoped<IGame, Game>();
            col.AddScoped<IGameBoard, GameBoard>();
            col.AddScoped<ITrackRenderer, TrackRenderer>();
            col.AddScoped<IBoardRenderer, GridRenderer>();
            col.AddScoped<IBoardRenderer, TrackLayoutRenderer>();

            ServiceProvider serviceProvider = col.BuildServiceProvider();

            return new MainForm(serviceProvider.GetService<IGame>());
        }
    }
}
