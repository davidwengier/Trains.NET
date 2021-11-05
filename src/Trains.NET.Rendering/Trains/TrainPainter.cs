using System;
using System.Collections.Generic;
using Trains.NET.Engine;

namespace Trains.NET.Rendering.Trains;

public class TrainPainter : ITrainPainter
{
    private readonly Dictionary<int, TrainPalette> _paletteMap = new();

    private static readonly TrainPalette s_baseTrainPalette = new(
        Colors.Black,
        Colors.VeryDarkGray,
        Colors.Gray,
        Colors.DarkBlue, // Had to pick one, blue won out!
        Colors.LightBlue // This is never used though.
    );

    public TrainPalette GetPalette(Train train)
    {
        if (!_paletteMap.ContainsKey(train.Seed))
        {
            _paletteMap.Add(train.Seed, GetPalette(train.GetPRNG()));
        }
        return _paletteMap[train.Seed];
    }

    private static TrainPalette GetPalette(BasicPRNG r)
    {
        byte sR = (byte)r.Next(32, 192);
        byte sG = (byte)r.Next(32, 192);
        byte sB = (byte)r.Next(32, 192);

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
        => new("#" + BitConverter.ToString(new[] { r, g, b }).Replace("-", string.Empty));
}
