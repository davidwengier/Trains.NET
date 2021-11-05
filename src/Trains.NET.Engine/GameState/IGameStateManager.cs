namespace Trains.NET.Engine;

public interface IGameStateManager
{
    void Load(int columns, int rows);
    void Save();
    void Reset(int columns, int rows);
}
