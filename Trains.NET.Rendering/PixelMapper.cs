namespace Trains.NET.Rendering
{
    public class PixelMapper : IPixelMapper
    {
        private readonly ITrackParameters _parameters;

        public PixelMapper(ITrackParameters parameters)
        {
            _parameters = parameters;
        }

        public (int, int) PixelsToCoords(int x, int y)
        {
            return (x / _parameters.CellSize, y / _parameters.CellSize);
        }

        public (int, int) CoordsToPixels(int column, int row)
        {
            return (column * _parameters.CellSize, row * _parameters.CellSize);
        }
    }
}
