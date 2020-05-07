using System.Linq;
using System.Windows;
using Trains.NET.Engine;

namespace Trains.NET.WPF
{
    [Order(50)]
    internal class GetShareCodeCommand : ICommand
    {
        private readonly IGameBoard _gameBoard;
        private readonly ITrackCodec _trackCodec;

        public GetShareCodeCommand(IGameBoard gameBoard, ITrackCodec trackCodec)
        {
            _gameBoard = gameBoard;
            _trackCodec = trackCodec;
        }

        public string Name => "Get Share Code";

        public void Execute()
        {
            var tracks = _gameBoard.GetTracks().ToList();

            string code = _trackCodec.Encode(_gameBoard.GetTracks().Select(t => t.Item3));

            Clipboard.SetText(code);
            MessageBox.Show("Your share code is:\n\n" + code + "\n\nIt has been copied to the clipboard.");
        }
    }
}

