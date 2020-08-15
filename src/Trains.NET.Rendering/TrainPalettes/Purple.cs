namespace Trains.NET.Rendering.TrainPalettes
{
    public class Purple : ITrainPalette
    {
        public Color OutlineColor => Colors.Black;

        public Color RearSectionStartColor => Colors.VeryDarkGray;

        public Color RearSectionEndColor => Colors.Gray;

        public Color FrontSectionStartColor => Colors.DarkPurple;

        public Color FrontSectionEndColor => Colors.LightPurple;
    }
}
