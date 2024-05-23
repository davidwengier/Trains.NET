namespace Trains.NET.Engine;

[Order(150)]
public class SaveCommand(IGameStateManager gameStateManager) : ICommand
{
    private readonly IGameStateManager _gameStateManager = gameStateManager;

    public string Name => "Save";

    public void Execute()
    {
        _gameStateManager.Save();
    }
}
