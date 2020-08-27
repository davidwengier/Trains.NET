using System;

namespace Trains.NET.Rendering
{
    public interface IGameParameters
    {
        int CellSize { get; }

        float GameScale { get; set; }

        event EventHandler GameScaleChanged;
    }
}
