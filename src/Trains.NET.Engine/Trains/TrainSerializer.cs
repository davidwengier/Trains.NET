using System.Diagnostics.CodeAnalysis;

namespace Trains.NET.Engine.Trains;

public class TrainSerializer : IEntitySerializer
{
    public bool TryDeserialize(string data, [NotNullWhen(true)] out IEntity? entity)
    {
        entity = null;
        var bits = data.Split('|', 10);
        if (bits.Length != 10)
        {
            return false;
        }

        if (!bits[0].Equals(nameof(Train)))
        {
            return false;
        }

        int i = 1;
        entity = new Train(int.Parse(bits[i++]))
        {
            CurrentSpeed = float.Parse(bits[i++]),
            Angle = float.Parse(bits[i++]),
            DesiredSpeed = float.Parse(bits[i++]),
            Follow = bool.Parse(bits[i++]),
            RelativeLeft = float.Parse(bits[i++]),
            RelativeTop = float.Parse(bits[i++]),
            Stopped = bool.Parse(bits[i++]),
            Carriages = int.Parse(bits[i++])
        };
        return true;
    }

    public bool TrySerialize(IEntity entity, [NotNullWhen(true)] out string? data)
    {
        data = null;
        if (entity is not Train)
        {
            return false;
        }

        var train = (Train)entity;

        data = $"{nameof(Train)}|{train.Seed}|{train.CurrentSpeed}|{train.Angle}|{train.DesiredSpeed}|{train.Follow}|{train.RelativeLeft}|{train.RelativeTop}|{train.Stopped}|{train.Carriages}";
        return true;
    }
}
