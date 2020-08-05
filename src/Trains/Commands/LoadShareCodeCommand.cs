using System;
using System.Collections.Generic;
using System.Windows;
using Trains.NET.Engine;

namespace Trains.Commands
{
    [Order(100)]
    internal class LoadShareCodeCommand : ICommand
    {
        private readonly ILayout _trackLayout;
        private readonly ITrackCodec _trackCodec;

        public LoadShareCodeCommand(ILayout trackLayout, ITrackCodec trackCodec)
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
                IEnumerable<IStaticEntity> tracks = _trackCodec.Decode(code);

                _trackLayout.Set(tracks);
            }
            catch (FormatException ex)
            {
                MessageBox.Show("An error occurred reading the code on the clipboard:\n\n" + ex.Message);
            }
        }
    }
}
