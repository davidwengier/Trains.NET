using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trains.NET.Engine
{
    internal class AsciiArtTrackSerializer : ITrackSerializer
    {
        private static readonly Dictionary<TrackDirection, char> s_trackMapping = new Dictionary<TrackDirection, char>()
        {
            { TrackDirection.Horizontal,   '═' },
            { TrackDirection.Vertical,     '║' },
            { TrackDirection.LeftUp,       '╝' },
            { TrackDirection.RightUp,      '╚' },
            { TrackDirection.RightDown,    '╔' },
            { TrackDirection.LeftDown,     '╗' },
            { TrackDirection.RightUpDown,  '╠' },
            { TrackDirection.LeftRightDown,'╦' },
            { TrackDirection.LeftUpDown,   '╣' },
            { TrackDirection.LeftRightUp,  '╩' },
            { TrackDirection.Cross,        '╬' },

            { TrackDirection.Undefined,    '?' }
        };

        public IEnumerable<Track> Deserialize(string[] lines)
        {
            List<Track> tracks = new List<Track>();

            for (int r = 0; r < lines.Length; r++)
            {
                for (int c = 0; c < lines[r].Length; c++)
                {
                    char current = lines[r][c];
                    var pair = s_trackMapping.FirstOrDefault(kvp => kvp.Value == current);

                    if (pair.Value != default)
                    {
                        var track = new Track(null);
                        track.Column = c;
                        track.Row = r;
                        track.Direction = pair.Key;
                        tracks.Add(track);
                    }
                }
            }

            return tracks;
        }

        public string Serialize(IEnumerable<Track> tracks)
        {
            StringBuilder sb = new StringBuilder();

            int maxColumn = tracks.Max(t => t.Column);
            int maxRow = tracks.Max(t => t.Row);

            for (int r = 0; r <= maxRow; r++)
            {
                for (int c = 0; c <= maxColumn; c++)
                {
                    Track track = tracks.FirstOrDefault(t => t.Column == c && t.Row == r);
                    if (track == null)
                    {
                        sb.Append(' ');
                    }
                    else
                    {
                        sb.Append(s_trackMapping[track.Direction]);
                    }
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}
