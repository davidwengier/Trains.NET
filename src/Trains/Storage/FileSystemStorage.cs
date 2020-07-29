using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Trains.NET.Engine;

namespace Trains.Storage
{
    internal class FileSystemStorage : IGameStorage
    {
        private const string Filename = "Trains.NET.tracks";

        public readonly string FilePath = GetFilePath(Filename);

        internal static string GetFilePath(string fileName)
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Trains.NET", fileName);
        }

        private readonly ITrackSerializer _serializer;

        public FileSystemStorage(ITrackSerializer serializer)
        {
            _serializer = serializer;
        }

        public IEnumerable<IStaticEntity> Read()
        {
            if (!File.Exists(FilePath))
            {
                return Enumerable.Empty<IStaticEntity>();
            }

            return _serializer.Deserialize(File.ReadAllLines(FilePath));
        }

        public void Write(IEnumerable<IStaticEntity> tracks)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
            File.WriteAllText(FilePath, _serializer.Serialize(tracks));
        }
    }
}

