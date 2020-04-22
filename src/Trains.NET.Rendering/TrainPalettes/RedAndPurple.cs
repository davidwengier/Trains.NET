namespace Trains.NET.Rendering.TrainPalettes
{
    internal class RedAndPurple : ITrainPalette
    {
        public Colors OutlineColor => Colors.Black;

        public Colors RearSectionStartColor => Colors.Black;

        public Colors RearSectionEndColor => Colors.Gray;

        public Colors FrontSectionStartColor => Colors.Purple;

        public Colors FrontSectionEndColor => Colors.Red;
    }
}
