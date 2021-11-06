namespace Trains.NET.Engine;

public interface IGameStorage
{
    string? Read(string key);
    void Write(string key, string value);
}
