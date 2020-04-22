namespace Trains.NET.Rendering.TrainPalettes
{
    internal class LegacyBlue : ITrainPalette
    {
        public Colors OutlineColor => Colors.Black;

        public Colors RearSectionStartColor => Colors.LegacyNearlyBlack;

        public Colors RearSectionEndColor => Colors.LegacyGray;

        public Colors FrontSectionStartColor => Colors.LegacyDarkBlue;

        public Colors FrontSectionEndColor => Colors.LegacyLightBlue;
    }
}
