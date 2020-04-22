namespace Trains.NET.Rendering
{
    public interface ITrainPalette
    {
        Colors OutlineColor { get; }
        Colors RearSectionStartColor { get; }
        Colors RearSectionEndColor { get; }
        Colors FrontSectionStartColor { get; }
        Colors FrontSectionEndColor { get; }
    }
}
