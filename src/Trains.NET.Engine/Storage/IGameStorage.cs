namespace Trains.NET.Engine;

public interface IGameStorage
{
    string? ReadEntities();
    void WriteEntities(string entities);
}
