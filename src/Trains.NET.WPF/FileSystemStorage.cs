using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Trains.NET.Engine;

namespace Trains.NET.WPF
{
    internal class FileSystemStorage : IGameStorage
    {
        public FileSystemStorage(ITrackSerializer trackSerializer)
        {
            _trackSerializer = trackSerializer;
        }

        private const string Filename = "Trains.NET.tracks";

        public readonly string FilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Trains.NET", Filename);

        private readonly ITrackSerializer _trackSerializer;

        public IEnumerable<Track> ReadTracks()
        {
            if (!File.Exists(FilePath))
            {
                return Enumerable.Empty<Track>();
            }
            string[] lines = File.ReadAllLines(FilePath);

            return _trackSerializer.Deserialize(lines);
        }

        public void WriteTracks(IEnumerable<Track> tracks)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath));

            File.WriteAllText(FilePath, _trackSerializer.Serialize(tracks));
        }
    }
}

