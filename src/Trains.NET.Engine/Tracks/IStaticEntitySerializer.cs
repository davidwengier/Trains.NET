using System.Diagnostics.CodeAnalysis;

namespace Trains.NET.Engine
{
    public interface IStaticEntitySerializer
    {
        bool TrySerialize(IStaticEntity entity, [NotNullWhen(true)] out string? data);

        bool TryDeserialize(string data, [NotNullWhen(true)] out IStaticEntity? entity);
    }
}
