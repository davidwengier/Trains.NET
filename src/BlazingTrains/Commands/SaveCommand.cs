using Trains.NET.Engine;

namespace BlazingTrains.Commands;

[Order(150)]

public class SaveCommand : ICommand
{
    public string Name => "Save";
    private readonly IGameStateManager _gameStateManager;

    public SaveCommand(IGameStateManager gameStateManager)
    {
        _gameStateManager = gameStateManager;
    }

    public void Execute()
    {
        _gameStateManager.Save();
    }
}
