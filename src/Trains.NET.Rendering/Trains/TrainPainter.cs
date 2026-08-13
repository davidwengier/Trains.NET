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
        var sR = (byte)r.Next(32, 192);
        var sG = (byte)r.Next(32, 192);
        var sB = (byte)r.Next(32, 192);

        var eR = (byte)(sR + 64);
        var eG = (byte)(sG + 64);
        var eB = (byte)(sB + 64);

        return s_baseTrainPalette with
        {
            FrontSectionStartColor = RGBToColor(sR, sG, sB),
            FrontSectionEndColor = RGBToColor(eR, eG, eB)
        };
    }

    private static Color RGBToColor(byte r, byte g, byte b)
        => new("#" + BitConverter.ToString(new[] { r, g, b }).Replace("-", string.Empty));
}
