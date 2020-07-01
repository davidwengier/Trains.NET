using System;
using System.Collections.Generic;
using System.Windows;
using Trains.NET.Engine;
using Trains.NET.Engine.Tracks;

namespace Trains.Commands
{
    [Order(100)]
    internal class LoadShareCodeCommand : ICommand
    {
        private readonly ITrackLayout _trackLayout;
        private readonly ITrackCodec _trackCodec;

        public LoadShareCodeCommand(ITrackLayout trackLayout, ITrackCodec trackCodec)
        {
            _trackLayout = trackLayout;
            _trackCodec = trackCodec;
        }

        public string Name => "Load Share Code";

        public void Execute()
        {
            string code = Clipboard.GetText();

            try
            {
                IEnumerable<Track> tracks = _trackCodec.Decode(code);

                _trackLayout.SetTracks(tracks);
            }
            catch (FormatException ex)
            {
                MessageBox.Show("An error occurred reading the code on the clipboard:\n\n" + ex.Message);
            }
        }
    }
}
