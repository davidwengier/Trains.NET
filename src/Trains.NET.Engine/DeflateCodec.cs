using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace Trains.NET.Engine
{
    internal class DeflateCodec : ITrackCodec
    {
        public IEnumerable<Track> Decode(string input)
        {
            var tracks = new List<Track>();

            if (input[0] != '1')
            {
                throw new FormatException("Code is not a version 1 share code.");
            }

            try
            {
                byte[] bytes = Convert.FromBase64String(input.Substring(1));
                using (var ms = new MemoryStream(bytes))
                using (var stream = new DeflateStream(ms, CompressionMode.Decompress))
                {
                    int col = stream.ReadByte();

                    while (col != -1)
                    {
                        int row = stream.ReadByte();
                        var direction = (TrackDirection)stream.ReadByte();

                        tracks.Add(new Track(null)
                        {
                            Column = col,
                            Row = row,
                            Direction = direction
                        });

                        col = stream.ReadByte();
                    }
                }

                return tracks;
            }
            catch
            {
                throw new FormatException("Code is not valid.");
            }
        }

        public string Encode(IEnumerable<Track> tracks)
        {
            var data = new List<byte>();

            foreach (Track? track in tracks)
            {
                // Long form, string method
                //sb.AppendFormat("{0:D3}{1:D3}{2:D2}", track.Column, track.Row, (int)track.Direction);

                // 4 byte, all in one int method
                //data.AddRange(BitConverter.GetBytes(Convert.ToInt32(string.Format("{0:D3}{1:D3}{2:D2}", track.Column, track.Row, (int)track.Direction))));

                // 3 byte (max 255 x 255 grid)
                data.Add((byte)track.Column);
                data.Add((byte)track.Row);
                data.Add((byte)(int)track.Direction);
            }

            using (var ms = new MemoryStream())
            {
                using (var stream = new DeflateStream(ms, System.IO.Compression.CompressionLevel.Optimal))
                {
                    stream.Write(data.ToArray(), 0, data.Count);
                }
                return "1" + Convert.ToBase64String(ms.ToArray());
            }
        }
    }
}
