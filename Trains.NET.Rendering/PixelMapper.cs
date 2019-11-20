using System;
using System.Collections.Generic;
using System.Text;

namespace Trains.NET.Rendering
{
    public class PixelMapper : IPixelMapper
    {
        public (int, int) PixelsToCoords(int x, int y)
        {
            return (x / Game.CellSize, y / Game.CellSize);
        }

        public (int, int) CoordsToPixels(int column, int row)
        {
            return (column * Game.CellSize, row * Game.CellSize);
        }
    }
}
