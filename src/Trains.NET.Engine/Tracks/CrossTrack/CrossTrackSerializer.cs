using System.Diagnostics.CodeAnalysis;

namespace Trains.NET.Engine.Tracks;

public class CrossTrackSerializer : IEntitySerializer
{
    public bool TryDeserialize(string data, [NotNullWhen(true)] out IEntity? entity)
    {
        entity = null;
        var bits = data.Split('.', 2);
        if (bits.Length != 2)
        {
            return false;
        }

        if (!bits[0].Equals(nameof(CrossTrack)))
        {
            return false;
        }

        var track = new CrossTrack()
        {
            Happy = bool.Parse(bits[1])
        };
        entity = track;
        return true;
    }

    public bool TrySerialize(IEntity entity, [NotNullWhen(true)] out string? data)
    {
        data = null;
        if (entity is not CrossTrack track)
        {
            return false;
        }

        data = $"{nameof(CrossTrack)}.{track.Happy}";
        return true;
    }
}
