using System;

namespace Trains.NET.Rendering;

public interface ITerrainMapRenderer
{
    event EventHandler? Changed;

    IImage GetTerrainImage();
}
