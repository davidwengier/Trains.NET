using System;
using System.Diagnostics.CodeAnalysis;

namespace Trains.NET.Engine.Tracks;

public class SignalSerializer : IEntitySerializer
{
    public bool TryDeserialize(string data, [NotNullWhen(true)] out IEntity? entity)
    {
        entity = null;
        var bits = data.Split('.', 5);
        if (bits.Length != 5)
        {
            return false;
        }

        if (!bits[0].Equals(nameof(Signal)))
        {
            return false;
        }

        var track = new Signal()
        {
            Direction = Enum.Parse<SingleTrackDirection>(bits[1]),
            SignalState = Enum.Parse<SignalState>(bits[2]),
            Happy = bool.Parse(bits[3]),
            TemporaryStopCounter = int.Parse(bits[4])
        };
        entity = track;
        return true;
    }

    public bool TrySerialize(IEntity entity, [NotNullWhen(true)] out string? data)
    {
        data = null;
        if (entity is not Signal signal)
        {
            return false;
        }

        data = $"{nameof(Signal)}.{signal.Direction}.{signal.SignalState}.{signal.Happy}.{signal.TemporaryStopCounter}";
        return true;
    }
}
