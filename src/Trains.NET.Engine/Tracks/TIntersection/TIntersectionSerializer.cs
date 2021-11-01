using System;
using System.Diagnostics.CodeAnalysis;

namespace Trains.NET.Engine.Tracks;

public class TIntersectionSerializer : IEntitySerializer
{
    public bool TryDeserialize(string data, [NotNullWhen(true)] out IEntity? entity)
    {
        entity = null;
        var bits = data.Split('.', 4);
        if (bits.Length != 4)
        {
            return false;
        }

        if (!bits[0].Equals(nameof(TIntersection)))
        {
            return false;
        }

        var track = new TIntersection()
        {
            Direction = Enum.Parse<TIntersectionDirection>(bits[1]),
            Style = Enum.Parse<TIntersectionStyle>(bits[2]),
            Happy = bool.Parse(bits[3])
        };
        entity = track;
        return true;
    }

    public bool TrySerialize(IEntity entity, [NotNullWhen(true)] out string? data)
    {
        data = null;
        if (entity.GetType() != typeof(TIntersection))
        {
            return false;
        }

        var track = (TIntersection)entity;

        data = $"{nameof(TIntersection)}.{track.Direction}.{track.Style}.{track.Happy}";
        return true;
    }
}
