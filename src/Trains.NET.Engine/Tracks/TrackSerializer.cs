using System;
using System.Diagnostics.CodeAnalysis;

namespace Trains.NET.Engine.Tracks
{
    public class TrackSerializer : IStaticEntitySerializer
    {
        public bool TryDeserialize(string data, [NotNullWhen(true)] out IStaticEntity? entity)
        {
            entity = null;
            var bits = data.Split('.', 4);
            if (bits.Length != 4)
            {
                return false;
            }

            if (!bits[0].Equals(nameof(Track)))
            {
                return false;
            }

            var track = new Track()
            {
                Direction = Enum.Parse<TrackDirection>(bits[1]),
                AlternateState = bool.Parse(bits[2]),
                Happy = bool.Parse(bits[3])
            };
            entity = track;
            return true;
        }

        public bool TrySerialize(IStaticEntity entity, [NotNullWhen(true)] out string? data)
        {
            data = null;
            if (entity.GetType() != typeof(Track))
            {
                return false;
            }

            var track = (Track)entity;

            data = $"{nameof(Track)}.{track.Direction}.{track.AlternateState}.{track.Happy}";
            return true;
        }
    }
}
