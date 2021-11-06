namespace Trains.NET.Engine;

[Order(10)]
public class ClearAllCommand : ICommand
{
    private readonly IGameStateManager _gameStateManager;

    public ClearAllCommand(IGameStateManager gameStateManager)
    {
        _gameStateManager = gameStateManager;
    }

    public string Name => "Clear All";

    public void Execute()
    {
        _gameStateManager.Reset();
    }
}
