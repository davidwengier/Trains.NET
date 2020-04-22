namespace Trains.NET.Rendering.TrainPalettes
{
    internal class GreenAndGold : ITrainPalette
    {
        public Colors OutlineColor => Colors.Black;

        public Colors RearSectionStartColor => Colors.Black;

        public Colors RearSectionEndColor => Colors.Gray;

        public Colors FrontSectionStartColor => Colors.Green;

        public Colors FrontSectionEndColor => Colors.Gold;
    }
}
