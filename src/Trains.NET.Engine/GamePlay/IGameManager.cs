namespace Trains.NET.Engine;

public interface IGameManager : IDisposable
{
    event EventHandler? Changed;

    ITool CurrentTool { get; set; }
    bool IsPaused { get; set; }
}
