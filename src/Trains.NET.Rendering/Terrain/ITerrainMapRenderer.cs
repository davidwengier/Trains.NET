using System.Diagnostics.CodeAnalysis;

namespace Trains.NET.Rendering
{
    public interface ITerrainMapRenderer
    {
        bool TryGetTerrainImage([NotNullWhen(true)] out IImage? image);
    }
}
