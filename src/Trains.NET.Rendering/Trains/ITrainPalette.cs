namespace Trains.NET.Rendering
{
    public interface ITrainPalette
    {
        Color OutlineColor { get; }
        Color RearSectionStartColor { get; }
        Color RearSectionEndColor { get; }
        Color FrontSectionStartColor { get; }
        Color FrontSectionEndColor { get; }
    }
}
