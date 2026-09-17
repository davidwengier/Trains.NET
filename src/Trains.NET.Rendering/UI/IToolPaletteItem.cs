using Trains.NET.Engine;

namespace Trains.NET.Rendering.UI;

public interface IToolPaletteItem
{
    ITool Tool { get; }

    ButtonBase Button { get; }
}
