namespace Trains.NET.Rendering.TrainPalettes
{
    internal class Yellow : ITrainPalette
    {
        public Color OutlineColor => Colors.Black;

        public Color RearSectionStartColor => Colors.VeryDarkGray;

        public Color RearSectionEndColor => Colors.Gray;

        public Color FrontSectionStartColor => Colors.DarkYellow;

        public Color FrontSectionEndColor => Colors.LightYellow;
    }
}
