using System.Diagnostics.CodeAnalysis;

namespace Trains.NET.Engine;

public class DepotSerializer : IEntitySerializer
{
    public bool TryDeserialize(string data, [NotNullWhen(true)] out IEntity? entity)
    {
        entity = null;
        var bits = data.Split('|', 4);
        if (bits.Length != 4 ||
            !bits[0].Equals(nameof(Depot)) ||
            !Enum.TryParse(bits[1], out DepotDirection direction) ||
            !int.TryParse(bits[2], out var trainSeed) ||
            !bool.TryParse(bits[3], out var hasProducedTrain))
        {
            return false;
        }

        entity = Depot.Rehydrate(trainSeed, direction, hasProducedTrain);
        return true;
    }

    public bool TrySerialize(IEntity entity, [NotNullWhen(true)] out string? data)
    {
        data = null;
        if (entity is not Depot depot)
        {
            return false;
        }

        data = $"{nameof(Depot)}|{depot.Direction}|{depot.TrainSeed}|{depot.HasProducedTrain}";
        return true;
    }
}
