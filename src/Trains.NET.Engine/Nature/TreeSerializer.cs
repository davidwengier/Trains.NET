using System.Diagnostics.CodeAnalysis;

namespace Trains.NET.Engine.Nature;

public class TreeSerializer : IEntitySerializer
{
    public bool TryDeserialize(string data, [NotNullWhen(true)] out IEntity? entity)
    {
        entity = null;
        var bits = data.Split('.', 2);
        if (bits.Length != 2)
        {
            return false;
        }

        if (!bits[0].Equals(nameof(Tree)))
        {
            return false;
        }

        entity = new Tree(int.Parse(bits[1]));
        return true;
    }

    public bool TrySerialize(IEntity entity, [NotNullWhen(true)] out string? data)
    {
        data = null;
        if (entity is not Tree tree)
        {
            return false;
        }

        data = $"{nameof(Tree)}.{tree.Seed}";
        return true;
    }
}
