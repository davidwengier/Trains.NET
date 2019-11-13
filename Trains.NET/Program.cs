using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
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

            using var form = CreateFormManual();

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
    }
}
