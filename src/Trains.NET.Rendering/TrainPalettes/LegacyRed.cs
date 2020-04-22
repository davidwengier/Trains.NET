namespace Trains.NET.Rendering.TrainPalettes
{
    internal class LegacyRed : ITrainPalette
    {
            public Colors OutlineColor => Colors.Black;

            public Colors RearSectionStartColor => Colors.LegacyNearlyBlack;

            public Colors RearSectionEndColor => Colors.LegacyGray;

            public Colors FrontSectionStartColor => Colors.LegacyDarkRed;

            public Colors FrontSectionEndColor => Colors.LegacyLightRed;
    }
}
