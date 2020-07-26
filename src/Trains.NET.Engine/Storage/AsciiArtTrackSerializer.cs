using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trains.NET.Engine
{
    internal class AsciiArtTrackSerializer : ITrackSerializer
    {
        private static readonly Dictionary<TrackDirection, char> s_trackMapping = new()
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

        private static readonly Dictionary<TrackDirection, char> s_alternateTrackMappings = new()
        {
            { TrackDirection.RightUpDown, '├' },
            { TrackDirection.LeftRightDown, '┬' },
            { TrackDirection.LeftUpDown, '┤' },
            { TrackDirection.LeftRightUp, '┴' }
        };



        public IEnumerable<Track> Deserialize(string[] lines)
        {
            var tracks = new List<Track>();

            for (int r = 0; r < lines.Length; r++)
            {
                for (int c = 0; c < lines[r].Length; c++)
                {
                    char current = lines[r][c];

                    bool alternate = false;
                    KeyValuePair<TrackDirection, char> pair = s_trackMapping.FirstOrDefault(kvp => kvp.Value == current);
                    if (pair.Value == '\0')
                    {
                        pair = s_alternateTrackMappings.FirstOrDefault(kvp => kvp.Value == current);
                        alternate = true;
                    }

                    if (pair.Value != default)
                    {
                        var track = new Track(null)
                        {
                            Column = c,
                            Row = r,
                            Direction = pair.Key,
                            AlternateState = alternate
                        };
                        tracks.Add(track);
                    }
                }
            }

            return tracks;
        }

        public string Serialize(IEnumerable<Track> tracks)
        {
            if (!tracks.Any()) return string.Empty;

            var sb = new StringBuilder();

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
                        Dictionary<TrackDirection, char>? mapping = track.AlternateState ? s_alternateTrackMappings : s_trackMapping;
                        sb.Append(mapping[track.Direction]);
                    }
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}
