namespace Trains.NET.Engine;

[Order(10)]
public class ClearAllCommand(IGameStateManager gameStateManager) : ICommand
{
    private readonly IGameStateManager _gameStateManager = gameStateManager;

    public string Name => "Clear All";

    public void Execute()
    {
        _gameStateManager.Reset();
    }
}
