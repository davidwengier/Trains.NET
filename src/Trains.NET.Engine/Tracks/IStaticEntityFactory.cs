using System.Diagnostics.CodeAnalysis;

namespace Trains.NET.Engine
{
    public interface IStaticEntityFactory<T> where T : IStaticEntity
    {
        bool TryCreateEntity(int column, int row, [NotNullWhen(returnValue: true)] out T? entity);
    }
}
