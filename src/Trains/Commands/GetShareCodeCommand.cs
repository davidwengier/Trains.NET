using System.Windows;
using Trains.NET.Engine;

namespace Trains.Commands
{
    [Order(50)]
    public class GetShareCodeCommand : ICommand
    {
        private readonly ILayout<Track> _trackLayout;
        private readonly ITrackCodec _trackCodec;

        public GetShareCodeCommand(ILayout<Track> trackLayout, ITrackCodec trackCodec)
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
