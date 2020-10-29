using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Trains.NET.Engine
{
    public interface IStaticEntityFactory<T> where T : IStaticEntity
    {
        IEnumerable<T> GetPossibleReplacements(int column, int row, T track);
        bool TryCreateEntity(int column, int row, [NotNullWhen(returnValue: true)] out T? entity);
    }
}
