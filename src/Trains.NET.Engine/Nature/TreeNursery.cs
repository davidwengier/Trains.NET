using System.Diagnostics.CodeAnalysis;

namespace Trains.NET.Engine.Nature
{
    public class TreeNursery : IStaticEntityFactory<Tree>
    {
        public bool TryCreateEntity(int column, int row, [NotNullWhen(true)] out Tree? entity)
        {
            entity = new Tree();
            return true;
        }
    }
}
