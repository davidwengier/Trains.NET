using System;
using System.Diagnostics.CodeAnalysis;

namespace Trains.NET.Engine.Tracks
{
    public class SingleTrackSerializer : IEntitySerializer
    {
        public bool TryDeserialize(string data, [NotNullWhen(true)] out IEntity? entity)
        {
            entity = null;
            var bits = data.Split('.', 3);
            if (bits.Length != 3)
            {
                return false;
            }

            if (!bits[0].Equals(nameof(SingleTrack)))
            {
                return false;
            }

            var track = new SingleTrack()
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
            if (entity.GetType() != typeof(SingleTrack))
            {
                return false;
            }

            var track = (SingleTrack)entity;

            data = $"{nameof(SingleTrack)}.{track.Direction}.{track.Happy}";
            return true;
        }
    }
}
