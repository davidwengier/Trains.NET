using System;
using System.Diagnostics.CodeAnalysis;

namespace Trains.NET.Engine.Tracks
{
    public class BridgeSerializer : IEntitySerializer
    {
        public bool TryDeserialize(string data, [NotNullWhen(true)] out IEntity? entity)
        {
            entity = null;
            var bits = data.Split('.', 3);
            if (bits.Length != 3)
            {
                return false;
            }

            if (!bits[0].Equals(nameof(Bridge)))
            {
                return false;
            }

            var track = new Bridge()
            {
                Direction = Enum.Parse<SingleTrackDirection>(bits[1]),
                Happy = bool.Parse(bits[2])
            };
            entity = track;
            return true;
        }

        public bool TrySerialize(IEntity entity, [NotNullWhen(true)] out string? data)
        {
            data = null;
            if (entity is not Bridge track)
            {
                return false;
            }

            data = $"{nameof(Bridge)}.{track.Direction}.{track.Happy}";
            return true;
        }
    }
}
