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

        private readonly string TracksFilePath = GetFilePath(TracksFilename);
        private readonly string TerrainFilePath = GetFilePath(TerrainFilename);

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
            if (!File.Exists(TracksFilePath))
            {
                return Enumerable.Empty<IStaticEntity>();
            }

            return _trackSerializer.Deserialize(File.ReadAllLines(TracksFilePath));
        }

        public void WriteStaticEntities(IEnumerable<IStaticEntity> tracks)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(TracksFilePath));
            File.WriteAllText(TracksFilePath, _trackSerializer.Serialize(tracks));
        }

        public IEnumerable<Terrain> ReadTerrain()
        {
            if (!File.Exists(TerrainFilePath))
            {
                return Enumerable.Empty<Terrain>();
            }

            return _terrainSerializer.Deserialize(File.ReadAllLines(TerrainFilePath));
        }

        public void WriteTerrain(IEnumerable<Terrain> terrainList)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(TerrainFilePath));
            File.WriteAllText(TerrainFilePath, _terrainSerializer.Serialize(terrainList));
        }
    }
}

