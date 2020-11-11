using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Trains.NET.Engine
{
    public class ExportToASCIICommand : ICommand
    {
        private static readonly Dictionary<TrackDirection, char> s_trackMapping = new()
        {
            { TrackDirection.Horizontal, '═' },
            { TrackDirection.Vertical, '║' },
            { TrackDirection.LeftUp, '╝' },
            { TrackDirection.RightUp, '╚' },
            { TrackDirection.RightDown, '╔' },
            { TrackDirection.LeftDown, '╗' },
            { TrackDirection.RightUp_RightDown, '╠' },
            { TrackDirection.RightDown_LeftDown, '╦' },
            { TrackDirection.LeftDown_LeftUp, '╣' },
            { TrackDirection.LeftUp_RightUp, '╩' },

            { TrackDirection.Undefined, '?' }
        };
        private readonly ILayout _layout;

        public ExportToASCIICommand(ILayout layout)
        {
            _layout = layout;
        }

        public string Name => "Export to ASCII";

        public void Execute()
        {
            if (!_layout.Any()) return;

            var sb = new StringBuilder();

            var happinessSb = new StringBuilder();

            int maxColumn = _layout.Max(t => t.Column);
            int maxRow = _layout.Max(t => t.Row);

            for (int r = 0; r <= maxRow; r++)
            {
                for (int c = 0; c <= maxColumn; c++)
                {
                    IStaticEntity? entity = _layout.FirstOrDefault(t => t.Column == c && t.Row == r);
                    if (entity == null)
                    {
                        sb.Append(' ');
                    }
                    else if (entity is Tree tree)
                    {
                        sb.Append('*');
                    }
                    else if (entity is Track track)
                    {
                        sb.Append(s_trackMapping[track.Direction]);
                    }
                }
                sb.AppendLine();
            }
            sb.AppendLine(happinessSb.ToString());

            Clipboard.SetText(sb.ToString());
        }
    }
}
