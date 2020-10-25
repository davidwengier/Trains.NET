using System;
using System.Collections.Generic;
using System.Text;

namespace Trains.NET.Engine.Storage
{
    public class GameSerializer : IEntityCollectionSerializer
    {
        private readonly IEnumerable<IEntitySerializer> _serializers;

        public GameSerializer(IEnumerable<IEntitySerializer> serializer)
        {
            _serializers = serializer;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public IEnumerable<IEntity> Deserialize(string[] lines)
        {
            List<IEntity> entities = new();

            foreach (var line in lines)
            {
                string[] bits = line.Split(new[] { '|' }, 3);
                foreach (var serializer in _serializers)
                {
                    try
                    {
                        if (serializer.TryDeserialize(bits[2], out var entity))
                        {
                            entity.Column = Convert.ToInt32(bits[0]);
                            entity.Row = Convert.ToInt32(bits[1]);
                            entities.Add(entity);
                        }
                    }
                    catch
                    {
                    }
                }
            }

            return entities;
        }

        public string Serialize(IEnumerable<IEntity> entities)
        {
            StringBuilder sb = new();

            foreach (IEntity entity in entities)
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
