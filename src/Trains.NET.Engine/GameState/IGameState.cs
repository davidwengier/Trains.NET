namespace Trains.NET.Engine;

public interface IGameState
{
    bool Load(IGameStorage storage);
    void Save(IGameStorage storage);
    void Reset();
}
