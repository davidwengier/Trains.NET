using System;
using System.Diagnostics.CodeAnalysis;

namespace Trains.NET.Engine;

public class ConfigSerializer : IEntitySerializer
{
    public bool TryDeserialize(string data, [NotNullWhen(true)] out IEntity? entity)
    {
        entity = null;
        var bits = data.Split('.', 3);
        if (bits.Length != 3)
        {
            return false;
        }

        if (!bits[0].Equals(nameof(ConfigEntity)))
        {
            return false;
        }

        entity = new ConfigEntity(bits[1], int.Parse(bits[2]));
        return true;
    }

    public bool TrySerialize(IEntity entity, [NotNullWhen(true)] out string? data)
    {
        data = null;
        if (entity is not ConfigEntity configEntity)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(configEntity.Name) ||
            configEntity.Name.IndexOf('.') > 0)
        {
            throw new Exception("Invalid config entity name, must not contain '.' or be empty.");
        }

        data = $"{nameof(ConfigEntity)}.{configEntity.Name}.{configEntity.Value}";
        return true;
    }
}
