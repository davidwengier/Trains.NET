using System;
using System.IO;
using Trains.NET.Engine;

namespace Trains.Storage;

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

    public string? ReadEntities()
    {
        if (!File.Exists(_tracksFilePath))
        {
            return null;
        }

        return File.ReadAllText(_tracksFilePath);
    }

    public void WriteEntities(string entities)
    {
        var tracksFileDirectory = Path.GetDirectoryName(_tracksFilePath);
        if (tracksFileDirectory != null)
        {
            Directory.CreateDirectory(tracksFileDirectory);
            File.WriteAllText(_tracksFilePath, entities);
        }
    }

    public int? ReadTerrainSeed()
    {
        if (!File.Exists(_terrainFilePath))
        {
            return null;
        }

        return Convert.ToInt32(File.ReadAllText(_terrainFilePath));
    }

    public void WriteTerrainSeed(int terrainSeed)
    {
        var terrainFileDirectory = Path.GetDirectoryName(_terrainFilePath);
        if (terrainFileDirectory != null)
        {
            Directory.CreateDirectory(terrainFileDirectory);
            File.WriteAllText(_terrainFilePath, terrainSeed.ToString());
        }
    }
}

