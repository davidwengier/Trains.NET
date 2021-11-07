using Trains.NET.Engine;

namespace BlazingTrains.Commands;

public class ChangeSaveModeCommand : ICommand
{
    private readonly IGameStateManager _gameStateManager;

    public ChangeSaveModeCommand(IGameStateManager gameStateManager)
    {
        _gameStateManager = gameStateManager;
    }

    public string Name => $"Change Auto Save Mode";

    public void Execute()
    {
        _gameStateManager.ChangeSaveMode();
    }
}
