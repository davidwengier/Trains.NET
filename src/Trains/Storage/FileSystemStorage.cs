using System;
using System.IO;
using Trains.NET.Engine;

namespace Trains.Storage;

public class FileSystemStorage : IGameStorage
{
    private const string TracksFilename = "Trains.NET.tracks";

    private readonly string _tracksFilePath = GetFilePath(TracksFilename);

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
}

