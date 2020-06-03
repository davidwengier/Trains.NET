using System;
using System.Collections.Generic;
using System.Windows;
using Trains.NET.Engine;

namespace Trains.Commands
{
    [Order(100)]
    internal class LoadShareCodeCommand : ICommand
    {
        private readonly IGameBoard _gameBoard;
        private readonly ITrackCodec _trackCodec;

        public LoadShareCodeCommand(IGameBoard gameBoard, ITrackCodec trackCodec)
        {
            _gameBoard = gameBoard;
            _trackCodec = trackCodec;
        }

        public string Name => "Load Share Code";

        public void Execute()
        {
            string code = Clipboard.GetText();

            try
            {
                IEnumerable<Track> tracks = _trackCodec.Decode(code);

                _gameBoard.LoadTracks(tracks);
            }
            catch (FormatException ex)
            {
                MessageBox.Show("An error occurred reading the code on the clipboard:\n\n" + ex.Message);
            }
        }
    }
}
