using System;
using System.Collections.Generic;
using Trains.NET.Engine;
using Trains.NET.Rendering;
using Trains.NET.Rendering.Trains;

namespace Trains.Emoji;

// -1 means it will be used instead of the in-game painter
[Order(-1)]
public class EmojiTrainPainter : ITrainPainter
{
    private static Random s_random = new(100);

    public static void Reset() => s_random = new Random(100);

    private readonly Dictionary<Train, TrainPalette> _paletteMap = new();

    public TrainPalette GetPalette(Train train)
    {
        if (!_paletteMap.ContainsKey(train))
        {
            _paletteMap.Add(train, GetRandomPalette());
        }
        return _paletteMap[train];
    }

    private static TrainPalette GetRandomPalette()
    {
        byte sR = (byte)s_random.Next(32, 192);
        byte sG = (byte)s_random.Next(32, 192);
        byte sB = (byte)s_random.Next(32, 192);

        byte eR = (byte)(sR + 64);
        byte eG = (byte)(sG + 64);
        byte eB = (byte)(sB + 64);

        return new TrainPalette(
            OutlineColor: Colors.Black,
            RearSectionStartColor: Colors.VeryDarkGray,
            RearSectionEndColor: Colors.Gray,
            FrontSectionStartColor: RGBToColor(sR, sG, sB),
            FrontSectionEndColor: RGBToColor(eR, eG, eB)
        );
    }

    private static Color RGBToColor(byte r, byte g, byte b)
        => new("#" + BitConverter.ToString(new[] { r, g, b }).Replace("-", string.Empty));
}
