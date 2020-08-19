using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Trains.NET.Engine;

namespace Trains.Storage
{
    public class FileSystemStorage : IGameStorage
    {
        private const string TracksFilename = "Trains.NET.tracks";
        private const string TerrainFilename = "Trains.NET.terrain";

        private readonly string _tracksFilePath = GetFilePath(TracksFilename);
        private readonly string _terrainFilePath = GetFilePath(TerrainFilename);

        internal static string GetFilePath(string fileName)
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Trains.NET", fileName);
        }

        private readonly ITrackSerializer _trackSerializer;
        private readonly ITerrainSerializer _terrainSerializer;

        public FileSystemStorage(ITrackSerializer serializer, ITerrainSerializer terrainSerializer)
        {
            _trackSerializer = serializer;
            _terrainSerializer = terrainSerializer;
        }

        public IEnumerable<IStaticEntity> ReadStaticEntities()
        {
            if (!File.Exists(_tracksFilePath))
            {
                return Enumerable.Empty<IStaticEntity>();
            }

            return _trackSerializer.Deserialize(File.ReadAllLines(_tracksFilePath));
        }

        public void WriteStaticEntities(IEnumerable<IStaticEntity> tracks)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_tracksFilePath));
            File.WriteAllText(_tracksFilePath, _trackSerializer.Serialize(tracks));
        }

        public IEnumerable<Terrain> ReadTerrain()
        {
            if (!File.Exists(_terrainFilePath))
            {
                return Enumerable.Empty<Terrain>();
            }

            return _terrainSerializer.Deserialize(File.ReadAllLines(_terrainFilePath));
        }

        public void WriteTerrain(IEnumerable<Terrain> terrainList)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_terrainFilePath));
            File.WriteAllText(_terrainFilePath, _terrainSerializer.Serialize(terrainList));
        }
    }
}

