namespace Trains.NET.Engine;

public interface IGameStateManager
{
    bool AutosaveEnabled { get; set; }

    void Load();
    void Save();
    void Reset();
}
