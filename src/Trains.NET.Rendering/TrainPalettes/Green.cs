namespace Trains.NET.Rendering.TrainPalettes
{
    public class Green : ITrainPalette
    {
        public Color OutlineColor => Colors.Black;

        public Color RearSectionStartColor => Colors.VeryDarkGray;

        public Color RearSectionEndColor => Colors.Gray;

        public Color FrontSectionStartColor => Colors.DarkGreen;

        public Color FrontSectionEndColor => Colors.LightGreen;
    }
}
