using System;
using System.Collections.Generic;
using System.Text;

namespace Trains.NET.Engine.Storage
{
    public class GameSerializer : ITrackSerializer
    {
        private readonly IEnumerable<IStaticEntitySerializer> _serializers;

        public GameSerializer(IEnumerable<IStaticEntitySerializer> serializer)
        {
            _serializers = serializer;
        }

        public IEnumerable<IStaticEntity> Deserialize(string[] lines)
        {
            List<IStaticEntity> entities = new();

            foreach (var line in lines)
            {
                string[] bits = line.Split('|', 3);
                foreach (var serializer in _serializers)
                {
                    if (serializer.TryDeserialize(bits[2], out var entity))
                    {
                        entity.Column = Convert.ToInt32(bits[0]);
                        entity.Row = Convert.ToInt32(bits[1]);
                        entities.Add(entity);
                    }
                }
            }

            return entities;
        }

        public string Serialize(IEnumerable<IStaticEntity> tracks)
        {
            StringBuilder sb = new();

            foreach (IStaticEntity entity in tracks)
            {
                foreach (var serializer in _serializers)
                {
                    if (serializer.TrySerialize(entity, out var data))
                    {
                        sb.Append(entity.Column);
                        sb.Append('|');
                        sb.Append(entity.Row);
                        sb.Append('|');
                        sb.AppendLine(data);
                    }
                }
            }

            return sb.ToString();
        }
    }
}
