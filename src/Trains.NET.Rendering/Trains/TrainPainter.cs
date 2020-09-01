using System;
using System.Collections.Generic;
using Trains.NET.Engine;

namespace Trains.NET.Rendering.Trains
{
    public class TrainPainter : ITrainPainter
    {
        private readonly Dictionary<Train, TrainPalette> _paletteMap = new();
        private readonly Random _random = new Random();

        private static readonly TrainPalette s_baseTrainPalette = new TrainPalette(
            Colors.Black,
            Colors.VeryDarkGray,
            Colors.Gray,
            Colors.DarkBlue, // Had to pick one, blue won out!
            Colors.LightBlue // This is never used though.
        );

        public TrainPalette GetPalette(Train train)
        {
            if (!_paletteMap.ContainsKey(train))
            {
                _paletteMap.Add(train, GetRandomPalette());
            }
            return _paletteMap[train];
        }

        private TrainPalette GetRandomPalette()
        {
            byte sR = (byte)_random.Next(32, 192);
            byte sG = (byte)_random.Next(32, 192);
            byte sB = (byte)_random.Next(32, 192);

            byte eR = (byte)(sR + 64);
            byte eG = (byte)(sG + 64);
            byte eB = (byte)(sB + 64);

            return s_baseTrainPalette with
            {
                FrontSectionStartColor = RGBToColor(sR, sG, sB),
                FrontSectionEndColor = RGBToColor(eR, eG, eB)
            };
        }

        private static Color RGBToColor(byte r, byte g, byte b)
            => new Color("#" + BitConverter.ToString(new[] { r, g, b }).Replace("-", string.Empty));
    }
}
