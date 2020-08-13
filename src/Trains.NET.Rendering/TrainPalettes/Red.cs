namespace Trains.NET.Rendering.TrainPalettes
{
    public class Red : ITrainPalette
    {
            public Color OutlineColor => Colors.Black;

            public Color RearSectionStartColor => Colors.VeryDarkGray;

            public Color RearSectionEndColor => Colors.Gray;

            public Color FrontSectionStartColor => Colors.DarkRed;

            public Color FrontSectionEndColor => Colors.LightRed;
    }
}
