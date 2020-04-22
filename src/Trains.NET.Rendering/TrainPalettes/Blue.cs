namespace Trains.NET.Rendering.TrainPalettes
{
    internal class Blue : ITrainPalette
    {
        public Color OutlineColor => Colors.Black;

        public Color RearSectionStartColor => Colors.VeryDarkGray;

        public Color RearSectionEndColor => Colors.Gray;

        public Color FrontSectionStartColor => Colors.DarkBlue;

        public Color FrontSectionEndColor => Colors.LightBlue;
    }
}
