namespace Trains.NET.Engine;

public interface IGameStateManager
{
    void Load();
    void Save();
    void Reset();
}
