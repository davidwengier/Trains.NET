using System.Windows;
using Trains.NET.Engine;
using Trains.NET.Engine.Tracks;

namespace Trains.Commands
{
    [Order(50)]
    internal class GetShareCodeCommand : ICommand
    {
        private readonly ITrackLayout _trackLayout;
        private readonly ITrackCodec _trackCodec;

        public GetShareCodeCommand(ITrackLayout trackLayout, ITrackCodec trackCodec)
        {
            _trackLayout = trackLayout;
            _trackCodec = trackCodec;
        }

        public string Name => "Get Share Code";

        public void Execute()
        {
            string code = _trackCodec.Encode(_trackLayout);

            Clipboard.SetText(code);
            MessageBox.Show("Your share code is:\n\n" + code + "\n\nIt has been copied to the clipboard.");
        }
    }
}
