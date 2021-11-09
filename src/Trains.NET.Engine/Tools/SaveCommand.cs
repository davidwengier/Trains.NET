namespace Trains.NET.Engine;

[Order(150)]
public class SaveCommand : ICommand
{
    private readonly IGameStateManager _gameStateManager;

    public string Name => "Save";

    public SaveCommand(IGameStateManager gameStateManager)
    {
        _gameStateManager = gameStateManager;
    }

    public void Execute()
    {
        _gameStateManager.Save();
    }
}
