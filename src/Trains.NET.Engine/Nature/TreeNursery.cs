using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Trains.NET.Engine.Nature;

public class TreeNursery : IStaticEntityFactory<Tree>
{
    // When we spawn a tree, pick a random seed!
    //  This doesn't need to be determanistic, as it will only be called on placement.
    //  As the seed is stored on the Tree, the tree style will persist between saves.
    private readonly Random _random = new();

    public IEnumerable<Tree> GetPossibleReplacements(int column, int row, Tree track)
    {
        yield return new Tree(_random.Next());
    }

    public bool TryCreateEntity(int column, int row, int fromColumn, int fromRow, [NotNullWhen(true)] out Tree? entity)
    {
        entity = new Tree(_random.Next());
        return true;
    }
}
