using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Trains.NET.Engine.Nature
{
    public class TreeNursery : IStaticEntityFactory<Tree>
    {
        public IEnumerable<Tree> GetPossibleReplacements(int column, int row, Tree track)
        {
            yield return new Tree();
        }

        public bool TryCreateEntity(int column, int row, bool isPartOfDrag, int fromColumn, int fromRow, [NotNullWhen(true)] out Tree? entity)
        {
            entity = new Tree();
            return true;
        }
    }
}
