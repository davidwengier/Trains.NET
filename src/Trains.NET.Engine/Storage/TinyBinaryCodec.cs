using System;
using System.Collections.Generic;
using System.Linq;

namespace Trains.NET.Engine
{
    public class TinyBinaryCodec : ITrackCodec
    {
        /*
         * Firstly, I'm sorry, this is not efficient, nice, or simple <3
         *  It works, and in the best bit-banging way!
         *  I'll rewrite this some day, but for now it will do.
         *  Take this, you may need it:  string.Join("", br._bytes.Select(x=>Convert.ToString(x, 2).PadLeft(8, '0')))
         */

        /*
         * Spec V1:
            [0 Track  ][0 empty ] - Declares the current cell is empty, moves to next cell
            [0 Track  ][1 track ][xxxx track dir  ] - Declares the current cell contains a particular track (as table below), moves to next cell
            [1 Control][0 repeat][xxx  count      ] - Repeats the following command x times
            [1 Control][1 misc  ][0    end of line] - Declares the line is over, move to next row & set col to 0
            [1 Control][1 misc  ][1    swap col/row or block] - *Experimental* Does literally nothing

            rqCt3GdQTuJ6gmJiYmJ6gmLuNiYnqCYmJiYnqCdxPUFruNA=
        */
        private const int MaxRepeat = 7;

        public IEnumerable<Track> Decode(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return Enumerable.Empty<Track>();

            var tracks = new List<Track>();

            var br = new BitReader(Convert.FromBase64String(input));

            int col = 0;
            int row = 0;
            int count = 1;
            
            while(!br.EndOfBytes())
            {
                if(br.ReadBit() == 0)
                {
                    // Base64 likes to pad extra 0's, so just to be safe!
                    if (br.EndOfBytes()) break;

                    // Track
                    if (br.ReadBit() == 0)
                    {
                        // Empty
                        col += count;
                    }
                    else
                    {
                        // Track Piece
                        var dir = (TrackEncoding)br.Read4BitInt();

                        for (int i=0; i<count; i++)
                        {
                            tracks.Add(new Track(null)
                            {
                                Column = col++,
                                Row = row,
                                Direction = s_directionMap[dir]
                            });
                        }
                    }
                    count = 1;
                }
                else
                {
                    // Control
                    if (br.ReadBit() == 0)
                    {
                        // Repeat
                        count = br.Read3BitInt();
                    }
                    else
                    {
                        // Misc
                        if (br.ReadBit() == 0)
                        {
                            // End Of Line
                            col = 0;
                            row += count;
                            count = 1;
                        }
                        else
                        {
                            // Experimental
                            throw new Exception("Potato is a fruit, implement this please <3");
                        }
                    }
                }
            }
            return tracks;
        }

        public string Encode(IEnumerable<Track> tracks)
        {
            if (!tracks.Any()) return string.Empty;

            var bw = new BitWriter();

            var rowGroups = tracks.GroupBy(x => x.Row).ToDictionary(x => x.Key, x => x.Select(x=>x));
            int lastRow = rowGroups.Keys.Max();

            int eolCounter = 0;

            // For each row
            for (int i = 0; i <= lastRow; i++)
            {
                // If we don't have anything, EOL it!
                if (!rowGroups.ContainsKey(i) || !rowGroups[i].Any())
                {
                    eolCounter++;
                    continue;
                }

                // If we had an EOL count, output it
                if (eolCounter > 0)
                {
                    // EOL is 3, RepeatEOL is 5 + 3, Only saves if we more than 2
                    WriteMultiple(bw, eolCounter, 2, x => x.WriteEOL());
                    eolCounter = 0;
                }

                EncodeRow(bw, rowGroups[i]);
                eolCounter++;
            }

            return Convert.ToBase64String(bw.ToArray());
        }

        private static void EncodeRow(BitWriter bw, IEnumerable<Track> rowTracks)
        {
            // Go through this row
            var row = rowTracks.ToDictionary(x => x.Column, x => x.Direction);
            int lastCol = row.Keys.Max();

            TrackEncoding lastDir = TrackEncoding.Blank;
            int lastDirCount = 0;

            for (int c = 0; c <= lastCol; c++)
            {
                TrackEncoding currentDir = TrackEncoding.Blank;
                if (row.ContainsKey(c))
                {
                    currentDir = s_directionMap.Single(x => x.Value == row[c]).Key;
                }
                if (currentDir == lastDir)
                {
                    lastDirCount++;
                }
                else
                {
                    if (lastDirCount > 0)
                    {
                        WriteOutTrack(bw, lastDir, lastDirCount);
                    }
                    lastDirCount = 1;
                    lastDir = currentDir;
                }
            }
            if (lastDirCount > 0)
            {
                WriteOutTrack(bw, lastDir, lastDirCount);
            }
        }

        private static void WriteOutTrack(BitWriter bw, TrackEncoding direction, int count)
        {
            if (direction == TrackEncoding.Blank)
            {
                // Empty is 2, RepeatEmpty is 5 + 2, Only saves if we more than 3
                WriteMultiple(bw, count, 3, x => x.WriteEmpty());
            }
            else
            {
                // Track is 6, RepeatTrack is 5 + 6, Only saves if we more than 1
                WriteMultiple(bw, count, 1, x => x.WriteTrack(direction));
            }
        }

        private static void WriteMultiple(BitWriter bw, int count, int minRepeat, Action<BitWriter> write)
        {
            while (count > 0)
            {
                // Maximum per repeat
                if (count > MaxRepeat)
                {
                    bw.WriteRepeat(MaxRepeat);
                    write(bw);
                    count -= MaxRepeat;
                }
                else if (count > minRepeat)
                {
                    bw.WriteRepeat(count);
                    write(bw);
                    break;
                }
                else
                {
                    for (int i = 0; i < count; i++)
                    {
                        write(bw);
                    }
                    break;
                }
            }
        }

        private static readonly Dictionary<TrackEncoding, TrackDirection> s_directionMap = new Dictionary<TrackEncoding, TrackDirection>
        {
            {TrackEncoding.Blank, TrackDirection.Undefined},
            {TrackEncoding.Horizontal, TrackDirection.Horizontal},
            {TrackEncoding.Vertical, TrackDirection.Vertical},
            {TrackEncoding.LeftUp, TrackDirection.LeftUp},
            {TrackEncoding.RightUp, TrackDirection.RightUp},
            {TrackEncoding.RightDown, TrackDirection.RightDown},
            {TrackEncoding.LeftDown, TrackDirection.LeftDown},
            {TrackEncoding.RightUpDown, TrackDirection.RightUpDown},
            {TrackEncoding.LeftRightDown, TrackDirection.LeftRightDown},
            {TrackEncoding.LeftUpDown, TrackDirection.LeftUpDown},
            {TrackEncoding.LeftRightUp, TrackDirection.LeftRightUp},
            {TrackEncoding.Cross, TrackDirection.Cross}
        };

        private enum TrackEncoding : ushort
        {
            Blank = 0b0000,
            Horizontal = 0b1100,
            Vertical = 0b0011,
            LeftUp = 0b1010,
            RightUp = 0b0110,
            RightDown = 0b0101,
            LeftDown = 0b1001,
            RightUpDown = 0b0111,
            LeftRightDown = 0b1101,
            LeftUpDown = 0b1011,
            LeftRightUp = 0b1110,
            Cross = 0b1111
        }

        private class BitReader
        {
            private const int BitIndexReset = 7;
            private int _byteIndex;
            private int _bitIndex = BitIndexReset;
            private readonly byte[] _bytes;

            public BitReader(byte[] b)
            {
                _bytes = b;
            }
            public int ReadBit()
            {
                int x = (_bytes[_byteIndex] >> _bitIndex) & 1;
                if (--_bitIndex < 0)
                {
                    _bitIndex = BitIndexReset;
                    _byteIndex++;
                }
                return x;
            }
            public int Read3BitInt() => ReadBit() << 2 | ReadBit() << 1 | ReadBit();
            public int Read4BitInt() => ReadBit() << 3 | ReadBit() << 2 | ReadBit() << 1 | ReadBit();
            public bool EndOfBytes() => _byteIndex >= _bytes.Length;
        }

        private class BitWriter
        {
            private readonly List<byte> _bytes = new List<byte>();
            private byte _currentByte;
            private int _bitIndex;

            public void Write3BitInt(int x) => Write(BitFromRight(x, 2), BitFromRight(x, 1), BitFromRight(x, 0));
            public void Write4BitInt(int x) => Write(BitFromRight(x, 3), BitFromRight(x, 2), BitFromRight(x, 1), BitFromRight(x, 0));
            public void WriteEOL() => Write(1, 1, 0);
            public void WriteEmpty() => Write(0, 0);
            public void WriteTrack(TrackEncoding track)
            {
                Write(0, 1);
                Write4BitInt((int)track);
            }
            public void WriteRepeat(int count)
            {
                if (count > MaxRepeat || count < 0) throw new ArgumentOutOfRangeException(nameof(count), "Repeat count out of range");
                Write(1, 0);
                Write3BitInt(count);
            }
            public byte[] ToArray()
            {
                if (_currentByte == 0) return _bytes.ToArray();
                return _bytes.Concat(new byte[] { (byte)(_currentByte << (7 - _bitIndex)) }).ToArray();
            }

            private void Write(params int[] bits)
            {
                for (int i = 0; i < bits.Length; i++)
                {
                    _currentByte |= (byte)(bits[i] & 1);

                    if (++_bitIndex > 7)
                    {
                        _bytes.Add(_currentByte);
                        _currentByte = 0;
                        _bitIndex = 0;
                    }
                    else
                    {
                        _currentByte <<= 1;
                    }
                }
            }
            private static int BitFromRight(int v, int index) => (v & (1 << index)) >> index;
        }
    }
}
