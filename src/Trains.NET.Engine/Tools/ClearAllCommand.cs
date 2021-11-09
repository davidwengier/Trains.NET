namespace Trains.NET.Engine;

[Order(10)]
public class ClearAllCommand : ICommand
{
    private readonly IGameStateManager _gameStateManager;

    public string Name => "Clear All";

    public ClearAllCommand(IGameStateManager gameStateManager)
    {
        _gameStateManager = gameStateManager;
    }

    public void Execute()
    {
        _gameStateManager.Reset();
    }
}
