using System.Diagnostics.CodeAnalysis;

namespace Trains.NET.Engine
{
    public interface IEntitySerializer
    {
        bool TrySerialize(IEntity entity, [NotNullWhen(true)] out string? data);

        bool TryDeserialize(string data, [NotNullWhen(true)] out IEntity? entity);
    }
}
