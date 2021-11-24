using Trains.NET.Engine;

namespace Trains.Storage;

public class FileSystemStorage : IGameStorage
{
    internal static string GetFilePath(string fileName)
    {
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Trains.NET", fileName);
    }

    public string? Read(string key)
    {
        var fileName = GetFilePath(key);
        if (!File.Exists(fileName))
        {
            return null;
        }

        return File.ReadAllText(fileName);
    }

    public void Write(string key, string value)
    {
        var fileName = GetFilePath(key);

        var directory = Path.GetDirectoryName(fileName);
        if (directory != null)
        {
            Directory.CreateDirectory(directory);
            File.WriteAllText(fileName, value);
        }
    }
}

