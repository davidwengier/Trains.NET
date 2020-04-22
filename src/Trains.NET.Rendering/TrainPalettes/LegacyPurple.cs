namespace Trains.NET.Rendering.TrainPalettes
{
    internal class LegacyPurple : ITrainPalette
    {
        public Colors OutlineColor => Colors.Black;

        public Colors RearSectionStartColor => Colors.LegacyNearlyBlack;

        public Colors RearSectionEndColor => Colors.LegacyGray;

        public Colors FrontSectionStartColor => Colors.LegacyDarkPurple;

        public Colors FrontSectionEndColor => Colors.LegacyLightPurple;
    }
}
