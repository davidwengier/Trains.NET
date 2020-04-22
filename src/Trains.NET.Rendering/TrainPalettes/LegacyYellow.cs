namespace Trains.NET.Rendering.TrainPalettes
{
    internal class LegacyYellow : ITrainPalette
    {
        public Colors OutlineColor => Colors.Black;

        public Colors RearSectionStartColor => Colors.LegacyNearlyBlack;

        public Colors RearSectionEndColor => Colors.LegacyGray;

        public Colors FrontSectionStartColor => Colors.LegacyDarkYellow;

        public Colors FrontSectionEndColor => Colors.LegacyLightYellow;
    }
}
