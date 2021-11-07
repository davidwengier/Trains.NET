using Trains.NET.Engine.Utilities;

namespace Trains.NET.Engine;

public interface IGameStateManager
{
    SaveModes SaveMode { get; }

    void Load();
    void Save();
    void Reset();
    void ChangeSaveMode();
}
