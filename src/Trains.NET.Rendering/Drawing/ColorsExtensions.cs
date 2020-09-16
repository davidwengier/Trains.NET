namespace Trains.NET.Rendering
{
    public static class ColorExtensions
    {
        public static Color WithAlpha(this Color colour, string alpha)
        {
            return new Color(colour.HexCode.Replace("#", $"#{alpha}"));
        }
    }
}
